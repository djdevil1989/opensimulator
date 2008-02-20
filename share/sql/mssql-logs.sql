SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [logs] (
  [logID] [int] NOT NULL,
  [target] [varchar](36) default NULL,
  [server] [varchar](64) default NULL,
  [method] [varchar](64) default NULL,
  [arguments] [varchar](255) default NULL,
  [priority] [int] default NULL,
  [message] [ntext],
  PRIMARY KEY CLUSTERED
(
	[logID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

