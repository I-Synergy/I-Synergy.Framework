using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Setup
{

    public enum Join
    {
        Inner,
        Left,
        Right,
        Outer
    }

    public enum InnerTable
    {
        Base,
        Side
    }

    public class SetupFilterOn
    {
        private Join _joinEnum;
        private string _tableName;
        private SetupFilter _filter;

        public SetupFilterOn()
        {

        }

        public SetupFilterOn(SetupFilter filter, Join joinEnum, string tableName)
        {
            _filter = filter;
            _joinEnum = joinEnum;
            _tableName = tableName;
        }

        public SetupFilterOn On(string leftTableName, string leftColumnName, string rightTableName, string rightColumnName)
        {
            var join = new SetupFilterJoin(_joinEnum, _tableName, leftTableName, leftColumnName, rightTableName, rightColumnName);
            _filter.AddJoin(join);
            return this;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();
    }


    [DataContract(Name = "sfj"), Serializable]
    public class SetupFilterJoin : SyncNamedItem<SetupFilterJoin>
    {
        [DataMember(Name = "je", IsRequired = true)]
        public Join JoinEnum { get; set; }

        [DataMember(Name = "tn", IsRequired = true)]
        public string TableName { get; set; }

        [DataMember(Name = "ltn", IsRequired = true)]
        public string LeftTableName { get; set; }

        [DataMember(Name = "lcn", IsRequired = true)]
        public string LeftColumnName { get; set; }

        [DataMember(Name = "rtn", IsRequired = true)]
        public string RightTableName { get; set; }

        [DataMember(Name = "rcn", IsRequired = true)]
        public string RightColumnName { get; set; }

        /// <summary>
        /// ctor for serializer, don't use as it, prefer the second ctor
        /// </summary>
        public SetupFilterJoin()
        {

        }

        public SetupFilterJoin(Join joinEnum, string tableName, string leftTableName, string leftColumnName, string rightTableName, string rightColumnName)
        {
            JoinEnum = joinEnum;
            TableName = tableName;
            LeftTableName = leftTableName;
            LeftColumnName = leftColumnName;
            RightTableName = rightTableName;
            RightColumnName = rightColumnName;
        }

        /// <summary>
        /// Get all comparable fields to determine if two instances are identifed are same by name
        /// </summary>
        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return JoinEnum.ToString();
            yield return TableName;
            yield return LeftColumnName;
            yield return LeftTableName;
            yield return RightColumnName;
            yield return RightTableName;
        }
    }
}
