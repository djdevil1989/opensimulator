/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.Addins;
using Mono.Addins.Setup;
using Mono.Addins.Description;
using OpenSim.Framework;
using Ux = OpenSim.Services.IntegrationService.IntegrationUtils;

namespace OpenSim.Services.IntegrationService
{
    // ****[ Robust ] These are the functions that connect our console
    // ****[ Robust ] commands to the addin management. This needs to
    // ****[ Robust ] be maintained in a file separately from the main
    // ****[ Robust ] implementation in the same namespace. Any generaly
    // ****[ Robust ] usefull methods can be located in one of the existing
    // ****[ Robust ] places
    // ****[ Robust ]
    // ****[ Robust ] This needs to be in OpenSim.Framework so it can be
    // ****[ Robust ] used to complete the region module management
    //
    // This will maintain the plugin repositories and plugins
    public class PluginManager : SetupService
    {
        protected AddinRegistry m_Registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenSim.Services.IntegrationService.PluginManager"/> class.
        /// </summary>
        /// <param name='r'>
        /// R.
        /// </param>
        internal PluginManager(AddinRegistry r): base (r)
        {
            m_Registry = r;
            m_Registry.Update();
        }

        /// <summary>
        /// Installs the plugin.
        /// </summary>
        /// <returns>
        /// The plugin.
        /// </returns>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public bool InstallPlugin(int ndx, out Dictionary<string, object> result)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            PackageCollection pack = new PackageCollection();
            PackageCollection toUninstall;
            DependencyCollection unresolved;

            IProgressStatus ps = new ConsoleProgressStatus(false);

            AddinRepositoryEntry[] available = GetSortedAvailbleAddins();

            if (ndx > (available.Length - 1))
            {
                MainConsole.Instance.Output("Selection out of range");
                result = res;
                return false;
            }

            AddinRepositoryEntry aentry = available[ndx];

            Package p = Package.FromRepository(aentry);
            pack.Add(p);

            ResolveDependencies(ps, pack, out toUninstall, out unresolved);

            // Attempt to install the plugin disabled
            if (Install(ps, pack) == true)
            {
                m_Registry.Update(ps);
                Addin addin = m_Registry.GetAddin(aentry.Addin.Id);
                m_Registry.DisableAddin(addin.Id);
                addin.Enabled = false;
                ListInstalledAddins(out res);
                result = res;
                return true;
            } 
            else
            {
                result = res;
                return false;
            }
        }

        // Remove plugin
        /// <summary>
        /// Uns the install.
        /// </summary>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public void UnInstall(int ndx)
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            if (ndx > (addins.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            Addin addin = addins[ndx];
            MainConsole.Instance.OutputFormat("Uninstalling plugin {0}", addin.Id);
            AddinManager.Registry.DisableAddin(addin.Id);
            addin.Enabled = false;
            IProgressStatus ps = new ConsoleProgressStatus(false);
            Uninstall(ps, addin.Id);
            return;
        }

        /// <summary>
        /// Checks the installed.
        /// </summary>
        /// <returns>
        /// The installed.
        /// </returns>
        public string CheckInstalled()
        {
            return "CheckInstall";
        }

        /// <summary>
        /// Lists the installed addins.
        /// </summary>
        /// <param name='result'>
        /// Result.
        /// </param>
        public void ListInstalledAddins(out Dictionary<string, object> result)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            int count = 0;
            foreach (Addin addin in addins)
            {
                Dictionary<string, object> r = new Dictionary<string, object>();
                r["enabled"] = addin.Enabled == true ? true : false;
                r["name"] = addin.LocalId;
                r["version"] = addin.Version;

                res.Add(count.ToString(), r);

                count++;
            }
            result = res;
            return;
        }

        // List compatible plugins in registered repositories
        /// <summary>
        /// Lists the available.
        /// </summary>
        /// <param name='result'>
        /// Result.
        /// </param>
        public void ListAvailable(out Dictionary<string, object> result)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            AddinRepositoryEntry[] addins = GetSortedAvailbleAddins();

            int count = 0;
            foreach (AddinRepositoryEntry addin in addins)
            {
                Dictionary<string, object> r = new Dictionary<string, object>();
                r["name"] = addin.Addin.Name;
                r["version"] = addin.Addin.Version;
                r["repository"] = addin.RepositoryName;

                res.Add(count.ToString(), r);
                count++;
            }
            result = res;
            return;
        }

        // List available updates ** 1
        /// <summary>
        /// Lists the updates.
        /// </summary>
        public void ListUpdates()
        {
            IProgressStatus ps = new ConsoleProgressStatus(true);
            Console.WriteLine ("Looking for updates...");
            Repositories.UpdateAllRepositories (ps);
            Console.WriteLine ("Available add-in updates:");
            bool found = false;
            AddinRepositoryEntry[] entries = Repositories.GetAvailableUpdates();

            foreach (AddinRepositoryEntry entry in entries)
            {
                Console.WriteLine(String.Format("{0}",entry.Addin.Id));
            }
        }

        // Sync to repositories
        /// <summary>
        /// Update this instance.
        /// </summary>
        public string Update()
        {
            IProgressStatus ps = new ConsoleProgressStatus(true);
            Repositories.UpdateAllRepositories(ps);
            return "Update";
        }

        // Register a repository
        /// <summary>
        /// Register a repository with our server.
        /// </summary>
        /// <returns>
        /// result of the action
        /// </returns>
        /// <param name='repo'>
        /// The URL of the repository we want to add
        /// </param>
        public bool AddRepository(string repo)
        {
            Repositories.RegisterRepository(null, repo, true);
            return true;
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        public void GetRepository()
        {
            Repositories.UpdateAllRepositories(new ConsoleProgressStatus(false));
        }

        // Remove a repository from the list
        /// <summary>
        /// Removes the repository.
        /// </summary>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public void RemoveRepository(string[] args)
        {
            AddinRepository[] reps = Repositories.GetRepositories();
            Array.Sort(reps, (r1,r2) => r1.Title.CompareTo(r2.Title));
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return;
            }

            int n = Convert.ToInt16(args[2]);
            if (n > (reps.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            AddinRepository rep = reps[n];
            Repositories.RemoveRepository(rep.Url);
            return;
        }

        // Enable repository
        /// <summary>
        /// Enables the repository.
        /// </summary>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public void EnableRepository(string[] args)
        {
            AddinRepository[] reps = Repositories.GetRepositories();
            Array.Sort(reps, (r1,r2) => r1.Title.CompareTo(r2.Title));
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return;
            }

            int n = Convert.ToInt16(args[2]);
            if (n > (reps.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            AddinRepository rep = reps[n];
            Repositories.SetRepositoryEnabled(rep.Url, true);
            return;
        }

        // Disable a repository
        /// <summary>
        /// Disables the repository.
        /// </summary>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public void DisableRepository(string[] args)
        {
            AddinRepository[] reps = Repositories.GetRepositories();
            Array.Sort(reps, (r1,r2) => r1.Title.CompareTo(r2.Title));
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return;
            }

            int n = Convert.ToInt16(args[2]);
            if (n > (reps.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            AddinRepository rep = reps[n];
            Repositories.SetRepositoryEnabled(rep.Url, false);
            return;
        }

        // List registered repositories
        /// <summary>
        /// Lists the repositories.
        /// </summary>
        /// <param name='result'>
        /// Result.
        /// </param>
        public void ListRepositories(out Dictionary<string, object> result)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            result = res;

            AddinRepository[] reps = GetSortedAddinRepo();
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return;
            }

            int count = 0;
            foreach (AddinRepository rep in reps)
            {
                Dictionary<string, object> r = new Dictionary<string, object>();
                r["enabled"] = rep.Enabled == true ? true : false;
                r["name"] = rep.Name;
                r["url"] = rep.Url;
                
                res.Add(count.ToString(), r);
                count++;
            }
            return;
        }

        /// <summary>
        /// Updates the registry.
        /// </summary>
        public void UpdateRegistry()
        {
            m_Registry.Update();
        }

        // Show plugin info
        /// <summary>
        /// Addins the info.
        /// </summary>
        /// <returns>
        /// The info.
        /// </returns>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public bool AddinInfo(int ndx, out Dictionary<string, object> result)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            result = res;

            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            if (ndx > (addins.Length - 1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return false;
            }
            // author category description
            Addin addin = addins[ndx];

            res["author"] = addin.Description.Author;
            res["category"] = addin.Description.Category;
            res["description"] = addin.Description.Description;
            res["name"] = addin.Name;
            res["url"] = addin.Description.Url;
            res["file_name"] = addin.Description.FileName;

            result = res;
            return true;
        }

        // Disable a plugin
        /// <summary>
        /// Disables the plugin.
        /// </summary>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public void DisablePlugin(string[] args)
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            int n = Convert.ToInt16(args[2]);
            if (n > (addins.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            Addin addin = addins[n];
            AddinManager.Registry.DisableAddin(addin.Id);
            addin.Enabled = false;
            return;
        }

        // Enable plugin
        /// <summary>
        /// Enables the plugin.
        /// </summary>
        /// <param name='args'>
        /// Arguments.
        /// </param>
        public void EnablePlugin(string[] args)
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            int n = Convert.ToInt16(args[2]);
            if (n > (addins.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            Addin addin = addins[n];

            addin.Enabled = true;
            AddinManager.Registry.EnableAddin(addin.Id);
            // AddinManager.Registry.Update();
            if(m_Registry.IsAddinEnabled(addin.Id))
            {
                ConsoleProgressStatus ps = new ConsoleProgressStatus(false);
                if (!AddinManager.AddinEngine.IsAddinLoaded(addin.Id))
                {
                    AddinManager.Registry.Rebuild(ps);
                    AddinManager.AddinEngine.LoadAddin(ps, addin.Id);
                }
            }
            else
            {
                MainConsole.Instance.OutputFormat("Not Enabled in this domain {0}", addin.Name);
            }
            return;
        }

        #region Util
        private void Testing()
        {
            Addin[] list = Registry.GetAddins();

            var addins = list.Where( a => a.Description.Category == "IntegrationPlugin");

            foreach (Addin addin in addins)
            {
                MainConsole.Instance.OutputFormat("Addin {0}", addin.Name);
            }
        }

        // These will let us deal with numbered lists instead
        // of needing to type in the full ids
        private AddinRepositoryEntry[] GetSortedAvailbleAddins()
        {
            ArrayList list = new ArrayList();
            list.AddRange(Repositories.GetAvailableAddins());

            AddinRepositoryEntry[] addins = list.ToArray(typeof(AddinRepositoryEntry)) as AddinRepositoryEntry[];

            Array.Sort(addins,(r1,r2) => r1.Addin.Id.CompareTo(r2.Addin.Id));

            return addins;
        }

        private AddinRepository[] GetSortedAddinRepo()
        {
            ArrayList list = new ArrayList();
            list.AddRange(Repositories.GetRepositories());

            AddinRepository[] repos = list.ToArray(typeof(AddinRepository)) as AddinRepository[];
            Array.Sort (repos,(r1,r2) => r1.Name.CompareTo(r2.Name));

            return repos;
        }

        private Addin[] GetSortedAddinList(string category)
        {
            ArrayList list = new ArrayList();
            list.AddRange(m_Registry.GetAddins());
            ArrayList xlist = new ArrayList();

            foreach (Addin addin in list)
            {
                if (addin.Description.Category == category)
                    xlist.Add(addin);
            }

            Addin[] addins = xlist.ToArray(typeof(Addin)) as Addin[];
            Array.Sort(addins,(r1,r2) => r1.Id.CompareTo(r2.Id));

            return addins;
        }
        #endregion Util

        #region Notes
        // ** 1 Not working
        #endregion Notes
    }
}