using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace LinguaSpace.Common.ComponentModel
{
    public class ProgressReportHelper
    {
        public static void Report(IProgressReport progress, ProgressType type, int count)
        {
            if (progress != null)
            {
                progress.Report(type, count);
            }
        }
    }

    public enum ProgressType
    {
        Start,
        Status,
        End
    }

    public class ProgressEventArgs : EventArgs
    {
        private ProgressType type;
        private int count;

        public ProgressEventArgs(ProgressType type, int count)
        {
            this.type = type;
            this.count = count;
        }

        public ProgressType Type
        {
            get
            {
                return this.type;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }
    }

    public interface IProgressReport
    {
        void Report(ProgressType type, int count);
    }

    public delegate void ProgressEventHandler(Object sender, ProgressEventArgs e);

    public interface IProgressTask
    {
        String Name
        {
            get;
        }

        String Description
        {
            get;
        }

        void Run();

        event ProgressEventHandler Progress;
    }

    public interface IProgress
    {
        IList<IProgressTask> Tasks
        {
            get;
        }

        void Run();
    }

    public class ProgressTask : IProgressReport, IProgressTask
    {
        private String name;
        private String description;

        public ProgressTask(String name, String description)
        {
            Debug.Assert(name != null);
            Debug.Assert(description != null);
            this.name = name;
            this.description = description;
        }

        public ProgressTask(String name)
            : this(name, String.Empty)
        {
        }

        public void Report(ProgressType type, int count)
        {
            if (this.Progress != null)
            {
                this.Progress(this, new ProgressEventArgs(type, count));
            }
        }

        public String Name
        {
            get
            {
                return this.name;
            }
        }

        public String Description
        {
            get
            {
                return this.description;
            }
        }

        public void Run()
        {
            if (this.RunBody != null)
            {
                this.RunBody(this, new EventArgs());
            }
        }

        public event EventHandler RunBody;
        public event ProgressEventHandler Progress;
    }
}