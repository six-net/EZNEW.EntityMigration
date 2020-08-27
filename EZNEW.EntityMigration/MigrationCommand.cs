using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;

namespace EZNEW.EntityMigration
{
    public class MigrationCommand : ICommand
    {
        public long Id => DateTime.Now.Ticks;

        public string CommandText { get; set; }
        public dynamic Parameters { get; set; }
        public string ObjectName { get; set; }
        public List<string> ObjectKeys { get; set; }
        public Dictionary<string, dynamic> ObjectKeyValues { get; set; }
        public List<string> ServerKeys { get; set; }
        public Dictionary<string, dynamic> ServerKeyValues { get; set; }
        public CommandExecuteMode ExecuteMode { get; set; }
        public IQuery Query { get; set; }
        public OperateType OperateType { get; set; }
        public IEnumerable<string> Fields { get; set; }

        public bool IsObsolete => throw new NotImplementedException();

        public Type EntityType { get; set; }

        public bool MustReturnValueOnSuccess { get; set; }

        public ICommand Clone()
        {
            return this;
        }

        public void ListenCallback(CommandCallbackEventHandler eventHandler, CommandCallbackEventParameter eventParameter)
        {
            throw new NotImplementedException();
        }

        public void ListenStarting(CommandStartingEventHandler eventHandler, CommandStartingEventParameter eventParameter, bool async = false)
        {
            throw new NotImplementedException();
        }

        public void TriggerCallbackEvent(bool success)
        {
            throw new NotImplementedException();
        }

        public CommandStartingEventExecuteResult TriggerStartingEvent()
        {
            throw new NotImplementedException();
        }
    }
}
