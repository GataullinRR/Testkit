using System;

namespace WebNotificationService.API
{
    public enum EntryType
    {
        Test,
        TestResult
    }

    public enum Change
    {
        Created,
        Modified,
        Deleted,
    }

    public abstract class EntryChangedWebMessageBase
    {
        public EntryType Entry { get; }
        public Change Change { get; }

        protected EntryChangedWebMessageBase(EntryType entry, Change change)
        {
            Entry = entry;
            Change = change;
        }
    };
}
