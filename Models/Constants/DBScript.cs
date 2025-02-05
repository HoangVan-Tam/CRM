using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Constants
{
    public static partial class Constants
    {
        public const string DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD = "SET ANSI_NULLS ON " +
                "GO " +
                "SET QUOTED_IDENTIFIER ON " +
                "GO " +
                "CREATE TABLE [dbo].[BC_230101_KEYWORD]( " +
                "[EntryID][int] IDENTITY(1, 1) NOT NULL, " +
                "[DateEntry] [datetime2] (7) NOT NULL, " +
                "[IsValid] [bit] NOT NULL, " +             
                "[Reason] [varchar] (250) NOT NULL, " +
                "[Response] [nvarchar] (500) NOT NULL, " +
                "[VerificationCode] [varchar] (20) NOT NULL, " +
                "[Chances] [int] NOT NULL, " +
                "[EntrySource] [varchar] (50) NOT NULL, " +               
                "AddMoreColumn" +
                "CONSTRAINT [PK_BC_230101_KEYWORD] PRIMARY KEY CLUSTERED " +
                "( " +
                "[EntryID] ASC " +
                ") WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] " +
                "GO; ";

        public const string DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD_Winner = "SET ANSI_NULLS ON" +
            "GO " +
            "SET QUOTED_IDENTIFIER ON " +
            "GO " +
            "CREATE TABLE[dbo].[BC_230101_KEYWORD_Winners] " +
            "( " +
            "[WinnerID][int] IDENTITY(1,1) NOT NULL, " +
            "[GroupName] [nvarchar] (250) NOT NULL, " +
            "[DateWon] [datetime] " +
            "NOT NULL, " +
            "[EntryID] [int] NOT NULL, " +
            "[MobileNo] [varchar] (250) NOT NULL, " +
            "CONSTRAINT[PK_BC_230101_KEYWORD_Winners] PRIMARY KEY CLUSTERED " +
            "( " +
            "[WinnerID] ASC " +
            ")WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON[PRIMARY] " +
            ") ON[PRIMARY] " +
            "GO " +
            "ALTER TABLE[dbo].[BC_230101_KEYWORD_Winners] WITH CHECK ADD CONSTRAINT[FK_BC_230101_KEYWORD_EntryID] FOREIGN KEY([EntryID]) " +
            "REFERENCES[dbo].[BC_230101_KEYWORD] " +
            "([EntryID]) " +
            "GO " +
            "ALTER TABLE[dbo].[BC_230101_KEYWORD_Winners] " +
            "CHECK CONSTRAINT[FK_BC_230101_KEYWORD_EntryID] " +
            "GO ";

        public const string DBSCRIPT_GET_ALL_ENTRIES = "SELECT * FROM [dbo].[BC_230101_KEYWORD] ORDER BY [EntryID] OFFSET {SkipRow} row FETCH NEXT {TakeRow} ROWS ONLY; ";
        public const string DBSCRIPT_GET_ALL_ENTRIES_NOPAGING = "SELECT * FROM [dbo].[BC_230101_KEYWORD] ORDER BY [EntryID]; ";

        public const string DBSCRIPT_PURGE_SELECTED_ENTRIES = "DELETE FROM [dbo].[BC_230101_KEYWORD] WHERE EntryID IN ({entriesID})";

        public const string DBSCRIPT_PURGE_ALL_ENTRIES = "DELETE FROM [dbo].[BC_230101_KEYWORD]";
    }
}
