﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.Scanning
{
    public class ScannerTable : IScannerTable
    {
        public string Pipe
        {
            get;
            set;
        }

        public string Execute
        {
            get;
            set;
        }

        public string CommandSeparator
        {
            get;
            set;
        }

        public string CommandConcatenator
        {
            get;
            set;
        }

        public string EnvironmentSeparator
        {
            get;
            set;
        }

        public string Escaper 
        { 
            get; 
            set; 
        }

        public string ArgumentSeparator 
        { 
            get;
            set;
        }

        public string ArgumentExecutableStarter
        {
            get;
            set;
        }

        public string ArgumentExecutableStopper
        {
            get;
            set;
        }

        public ScannerTable()
        {
            Pipe = "|";
            Execute = "$";
            CommandSeparator = ";";
            EnvironmentSeparator = ".";
            CommandConcatenator = ",";
            Escaper = "\\";
            ArgumentSeparator = ":";
            ArgumentExecutableStarter = "(";
            ArgumentExecutableStopper = ")";
        }

        public ScannerTable(ScannerTable original)
        {
            Pipe = original.Pipe;
            Execute = original.Execute;
            CommandSeparator = original.CommandSeparator;
            EnvironmentSeparator = original.EnvironmentSeparator;
            CommandConcatenator = original.CommandConcatenator;
            Escaper = original.Escaper;
            ArgumentSeparator = original.ArgumentSeparator;
            ArgumentExecutableStarter = original.ArgumentExecutableStarter;
            ArgumentExecutableStopper = original.ArgumentExecutableStopper;
        }


        public IScannerTable Copy()
        {
            return new ScannerTable(this);
        }
    }
}
