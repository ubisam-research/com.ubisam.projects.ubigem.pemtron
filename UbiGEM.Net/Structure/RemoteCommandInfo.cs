using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Command Parameter 정보입니다.
    /// </summary>
    public class CommandParameterInfo
    {
        /// <summary>
        /// Command Parameter Name을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Command Parameter Format을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSItemFormat Format { get; set; }

        /// <summary>
        /// Command Parameter Value를 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSValue Value { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public CommandParameterInfo()
        {
            this.Name = string.Empty;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Name={0}, Format={1}, Value={2}",
                this.Name, this.Format, this.Value);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public CommandParameterInfo CopyTo()
        {
            CommandParameterInfo result;
            UbiCom.Net.Structure.SECSValue secsValue;

            secsValue = new UbiCom.Net.Structure.SECSValue();

            secsValue.SetValue(this.Value);

            result = new CommandParameterInfo()
            {
                Name = this.Name,
                Format = this.Format,
                Value = secsValue
            };

            return result;
        }
    }

    /// <summary>
    /// Command Parameter Collection 정보입니다.
    /// </summary>
    public class CommandParameterCollection
    {
        /// <summary>
        /// Command Parameter 정보를 가져오거나 설정합니다.
        /// </summary>
        public List<CommandParameterInfo> Items { get; set; }

        /// <summary>
        /// Command Parameter 정보를 가져옵니다.
        /// </summary>
        /// <param name="cpName">가져올 Command Paramter Name입니다.</param>
        /// <returns>Command Parameter 정보입니다.(단, 없을 경우 null)</returns>
        public CommandParameterInfo this[string cpName]
        {
            get { return this.Items.FirstOrDefault(t => t.Name == cpName); }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public CommandParameterCollection()
        {
            this.Items = new List<CommandParameterInfo>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Item Count={0}", this.Items.Count);
        }

        /// <summary>
        /// Command Parameter 정보를 추가합니다.
        /// </summary>
        /// <param name="commandParameterInfo">추가할 Command Parameter 정보입니다.</param>
        public void Add(CommandParameterInfo commandParameterInfo)
        {
            this.Items.Add(commandParameterInfo);
        }

        /// <summary>
        /// Command Parameter 정보를 삭제합니다.
        /// </summary>
        /// <param name="commandParameterInfo">삭제할 Command Parameter 정보입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(CommandParameterInfo commandParameterInfo)
        {
            bool result;

            result = false;

            var varInfo = (from CommandParameterInfo tempCommandParameterInfo in this.Items
                           where tempCommandParameterInfo.Name == commandParameterInfo.Name &&
                                 tempCommandParameterInfo.Format == commandParameterInfo.Format
                           select tempCommandParameterInfo).FirstOrDefault();

            if (varInfo != null)
            {
                result = this.Items.Remove(varInfo);
            }

            return result;
        }

        /// <summary>
        /// Command Parameter 정보를 삭제합니다.
        /// </summary>
        /// <param name="commandParameName">삭제할 Command Parameter Name입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(string commandParameName)
        {
            bool result;

            result = false;

            var varInfo = (from CommandParameterInfo tempCommandParameterInfo in this.Items
                           where tempCommandParameterInfo.Name == commandParameName
                           select tempCommandParameterInfo).FirstOrDefault();

            if (varInfo != null)
            {
                result = this.Items.Remove(varInfo);
            }

            return result;
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public CommandParameterCollection CopyTo()
        {
            CommandParameterCollection result;

            result = new CommandParameterCollection();

            this.Items.ForEach(t =>
            {
                result.Add(t.CopyTo());
            });

            return result;
        }
    }

    /// <summary>
    /// Remote Command 정보입니다.
    /// </summary>
    public class RemoteCommandInfo
    {
        /// <summary>
        /// Remote Command Name을 가져오거나 설정합니다.
        /// </summary>
        public string RemoteCommand { get; set; }

        /// <summary>
        /// Remote Command Description을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public uint SystemBytes { get; set; }

        /// <summary>
        /// Command Parameter 정보를 가져오거나 설정합니다.(S2F41)
        /// </summary>
        public CommandParameterCollection CommandParameter { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public RemoteCommandInfo()
        {
            this.RemoteCommand = string.Empty;
            this.Description = string.Empty;

            this.CommandParameter = new CommandParameterCollection();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Remote Command={0}, Description={1}, Command Parameter Count={2}",
                this.RemoteCommand, this.Description, this.CommandParameter.Items.Count);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public RemoteCommandInfo CopyTo()
        {
            RemoteCommandInfo result;

            result = new RemoteCommandInfo()
            {
                RemoteCommand = this.RemoteCommand,
                SystemBytes = this.SystemBytes,
                Description = this.Description,
                CommandParameter = this.CommandParameter.CopyTo()
            };

            return result;
        }
    }

    /// <summary>
    /// Enhanced Command Parameter Item 정보입니다.
    /// </summary>
    public class EnhancedCommandParameterItem
    {
        private UbiCom.Net.Structure.SECSItemFormat _format;

        /// <summary>
        /// Command Parameter Name을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Command Parameter Format을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSItemFormat Format
        {
            get { return this._format; }
            set
            {
                this._format = value;

                if (this._format == UbiCom.Net.Structure.SECSItemFormat.L)
                {
                    if (this.ChildParameterItem == null)
                    {
                        this.ChildParameterItem = new EnhancedCommandParameterInfo();
                    }
                }
            }
        }

        /// <summary>
        /// List일 경우 child command parameter type을 가져오거나 설정합니다.
        /// </summary>
        public CPType ParameterType { get; set; }

        /// <summary>
        /// Command Parameter Value를 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSValue Value { get; set; }

        /// <summary>
        /// Parameter Item의 하위 Parameter Item을 가져오거나 설정합니다.(Format=L일 경우)
        /// </summary>
        public EnhancedCommandParameterInfo ChildParameterItem { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public EnhancedCommandParameterItem()
        {
            this.Name = string.Empty;

            this.ChildParameterItem = null;
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            if (this.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                return string.Format("Name={0}, Format={1}, Parameter Count={2}", this.Name, this.Format, this.ChildParameterItem.Items.Count);
            }
            else
            {
                return string.Format("Name={0}, Format={1}, Value={2}", this.Name, this.Format, this.Value);
            }
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public EnhancedCommandParameterItem CopyTo()
        {
            EnhancedCommandParameterItem result;
            UbiCom.Net.Structure.SECSValue secsValue;

            secsValue = new UbiCom.Net.Structure.SECSValue();

            secsValue.SetValue(this.Value);

            result = new EnhancedCommandParameterItem()
            {
                Name = this.Name,
                Format = this.Format,
                ParameterType = this.ParameterType,
                Value = secsValue
            };

            if (this.ChildParameterItem != null)
            {
                result.ChildParameterItem = this.ChildParameterItem.CopyTo();
            }

            return result;
        }
    }

    /// <summary>
    /// Enhanced Command Parameter 정보입니다.
    /// </summary>
    public class EnhancedCommandParameterInfo
    {
        /// <summary>
        /// Command Parameter Name을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Command Parameter Format을 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSItemFormat Format { get; set; }

        /// <summary>
        /// List일 경우 child command parameter type을 가져오거나 설정합니다.
        /// </summary>
        public CPType ParameterType { get; set; }

        /// <summary>
        /// Command Parameter Value를 가져오거나 설정합니다.
        /// </summary>
        public UbiCom.Net.Structure.SECSValue Value { get; set; }

        /// <summary>
        /// Command Parameter Item을 가져오거나 설정합니다.
        /// </summary>
        public List<EnhancedCommandParameterItem> Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpName"></param>
        /// <returns></returns>
        public EnhancedCommandParameterItem this[string cpName]
        {
            get { return this.Items.FirstOrDefault(t => t.Name == cpName); }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public EnhancedCommandParameterInfo()
        {
            this.Name = string.Empty;
            this.Items = new List<EnhancedCommandParameterItem>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            if (this.Format == UbiCom.Net.Structure.SECSItemFormat.L)
            {
                return string.Format("Name={0}, Format={1}, Parameter Count={2}", this.Name, this.Format, this.Items.Count);
            }
            else
            {
                return string.Format("Name={0}, Format={1}, Value={2}", this.Name, this.Format, this.Value);
            }
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public EnhancedCommandParameterInfo CopyTo()
        {
            EnhancedCommandParameterInfo result;
            UbiCom.Net.Structure.SECSValue secsValue;

            secsValue = new UbiCom.Net.Structure.SECSValue();

            secsValue.SetValue(this.Value);

            result = new EnhancedCommandParameterInfo()
            {
                Name = this.Name,
                Format = this.Format,
                Value = secsValue
            };

            foreach (EnhancedCommandParameterItem tempEnhancedCommandParameterItem in this.Items)
            {
                result.Items.Add(tempEnhancedCommandParameterItem.CopyTo());
            }

            return result;
        }
    }

    /// <summary>
    /// Enhanced Command Parameter Collection 정보입니다.
    /// </summary>
    public class EnhancedCommandParameterCollection
    {
        /// <summary>
        /// Command Parameter 정보를 가져오거나 설정합니다.
        /// </summary>
        public List<EnhancedCommandParameterInfo> Items { get; set; }

        /// <summary>
        /// Command Parameter 정보를 가져옵니다.
        /// </summary>
        /// <param name="cpName">가져올 Command Paramter Name입니다.</param>
        /// <returns>Command Parameter 정보입니다.(단, 없을 경우 null)</returns>
        public EnhancedCommandParameterInfo this[string cpName]
        {
            get { return this.Items.FirstOrDefault(t => t.Name == cpName); }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public EnhancedCommandParameterCollection()
        {
            this.Items = new List<EnhancedCommandParameterInfo>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Item Count={0}", this.Items.Count);
        }

        /// <summary>
        /// Enhanced Command Parameter 정보를 추가합니다.
        /// </summary>
        /// <param name="commandParameterInfo">추가할 Command Parameter 정보입니다.</param>
        public void Add(EnhancedCommandParameterInfo commandParameterInfo)
        {
            this.Items.Add(commandParameterInfo);
        }

        /// <summary>
        /// Enhanced Command Parameter 정보를 삭제합니다.
        /// </summary>
        /// <param name="commandParameterInfo">삭제할 Command Parameter 정보입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(EnhancedCommandParameterInfo commandParameterInfo)
        {
            bool result;

            result = false;

            var varInfo = (from EnhancedCommandParameterInfo tempCommandParameterInfo in this.Items
                           where tempCommandParameterInfo.Name == commandParameterInfo.Name
                           select tempCommandParameterInfo).FirstOrDefault();

            if (varInfo != null)
            {
                result = this.Items.Remove(varInfo);
            }

            return result;
        }

        /// <summary>
        /// Enhanced Command Parameter 정보를 삭제합니다.
        /// </summary>
        /// <param name="commandParameName">삭제할 Command Parameter Name입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(string commandParameName)
        {
            bool result;

            result = false;

            var varInfo = (from EnhancedCommandParameterInfo tempCommandParameterInfo in this.Items
                           where tempCommandParameterInfo.Name == commandParameName
                           select tempCommandParameterInfo).FirstOrDefault();

            if (varInfo != null)
            {
                result = this.Items.Remove(varInfo);
            }

            return result;
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public EnhancedCommandParameterCollection CopyTo()
        {
            EnhancedCommandParameterCollection result;

            result = new EnhancedCommandParameterCollection();

            this.Items.ForEach(t =>
            {
                result.Add(t.CopyTo());
            });

            return result;
        }
    }

    /// <summary>
    /// Enhanced Remote Command 정보입니다.
    /// </summary>
    public class EnhancedRemoteCommandInfo
    {
        /// <summary>
        /// Remote Command Name을 가져오거나 설정합니다.
        /// </summary>
        public string RemoteCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public uint SystemBytes { get; set; }

        /// <summary>
        /// Remote Command Description을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data ID를 가져오거나 설정합니다.
        /// </summary>
        public string DataID { get; set; }

        /// <summary>
        /// Object Spec을 가져오거나 설정합니다.
        /// </summary>
        public string ObjSpec { get; set; }

        /// <summary>
        /// Enhanced Command Parameter 정보를 가져오거나 설정합니다.(S2F49)
        /// </summary>
        public EnhancedCommandParameterCollection EnhancedCommandParameter { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public EnhancedRemoteCommandInfo()
        {
            this.RemoteCommand = string.Empty;
            this.Description = string.Empty;
            this.ObjSpec = string.Empty;

            this.EnhancedCommandParameter = new EnhancedCommandParameterCollection();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Remote Command={0}, Description={1}, Enhanced Command Parameter Count={2}",
                this.RemoteCommand, this.Description, this.EnhancedCommandParameter.Items.Count);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>이 인스턴스의 복사본인 새 개체입니다.</returns>
        public EnhancedRemoteCommandInfo CopyTo()
        {
            EnhancedRemoteCommandInfo result;

            result = new EnhancedRemoteCommandInfo()
            {
                RemoteCommand = this.RemoteCommand,
                Description = this.Description,
                DataID = this.DataID,
                ObjSpec = this.ObjSpec,
                EnhancedCommandParameter = this.EnhancedCommandParameter.CopyTo()
            };

            return result;
        }
    }

    /// <summary>
    /// Remote Command Collection 정보입니다.
    /// </summary>
    public class RemoteCommandCollection
    {
        /// <summary>
        /// Remote Command 정보를 가져오거나 설정합니다.
        /// </summary>
        public List<RemoteCommandInfo> RemoteCommandItems { get; set; }

        /// <summary>
        /// Remote Command 정보를 가져오거나 설정합니다.
        /// </summary>
        public List<EnhancedRemoteCommandInfo> EnhancedRemoteCommandItems { get; set; }

        /// <summary>
        /// Remote Command 정보를 가져옵니다.
        /// </summary>
        /// <param name="rcmd">가져올 Remote Command입니다.</param>
        /// <returns>Remote Command 정보입니다.(단, 없을 경우 null)</returns>
        public RemoteCommandInfo this[string rcmd]
        {
            get { return this.RemoteCommandItems.FirstOrDefault(t => t.RemoteCommand == rcmd); }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public RemoteCommandCollection()
        {
            this.RemoteCommandItems = new List<RemoteCommandInfo>();
            this.EnhancedRemoteCommandItems = new List<EnhancedRemoteCommandInfo>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Item Count={0}, Enhanced Item Count={1}", this.RemoteCommandItems.Count, this.EnhancedRemoteCommandItems.Count);
        }

        /// <summary>
        /// Remote Command 정보를 추가합니다.
        /// </summary>
        /// <param name="remoteCommandInfo">추가할 Remote Command 정보입니다.</param>
        public void Add(RemoteCommandInfo remoteCommandInfo)
        {
            this.RemoteCommandItems.Add(remoteCommandInfo);
        }

        /// <summary>
        /// Enhanced Remote Command 정보를 추가합니다.
        /// </summary>
        /// <param name="remoteCommandInfo">추가할 Enhanced Remote Command 정보입니다.</param>
        public void Add(EnhancedRemoteCommandInfo remoteCommandInfo)
        {
            this.EnhancedRemoteCommandItems.Add(remoteCommandInfo);
        }

        /// <summary>
        /// Remote Command 정보를 삭제합니다.
        /// </summary>
        /// <param name="remoteCommandInfo">삭제할 Remote Command 정보입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(RemoteCommandInfo remoteCommandInfo)
        {
            bool result;

            result = false;

            var varInfo = (from RemoteCommandInfo tempRemoteCommandInfo in this.RemoteCommandItems
                           where tempRemoteCommandInfo.RemoteCommand == remoteCommandInfo.RemoteCommand &&
                                 tempRemoteCommandInfo.Description == remoteCommandInfo.Description
                           select tempRemoteCommandInfo).FirstOrDefault();

            if (varInfo != null)
            {
                result = this.RemoteCommandItems.Remove(varInfo);
            }

            return result;
        }

        /// <summary>
        /// Enhanced Remote Command 정보를 삭제합니다.
        /// </summary>
        /// <param name="remoteCommandInfo">삭제할 Enhanced Remote Command 정보입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(EnhancedRemoteCommandInfo remoteCommandInfo)
        {
            bool result;

            result = false;

            var varInfo = (from EnhancedRemoteCommandInfo tempRemoteCommandInfo in this.EnhancedRemoteCommandItems
                           where tempRemoteCommandInfo.RemoteCommand == remoteCommandInfo.RemoteCommand &&
                                 tempRemoteCommandInfo.Description == remoteCommandInfo.Description
                           select tempRemoteCommandInfo).FirstOrDefault();

            if (varInfo != null)
            {
                result = this.EnhancedRemoteCommandItems.Remove(varInfo);
            }

            return result;
        }

        /// <summary>
        /// Remote Command 정보를 삭제합니다.
        /// </summary>
        /// <param name="remoteCommand">삭제할 Remote Command입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다. 이 메서드는 VID가 없는 경우 false를 반환합니다.</returns>
        public bool Remove(string remoteCommand)
        {
            bool result;
            bool exist;

            result = false;

            exist = this.RemoteCommandItems.Any(t => (t.RemoteCommand == remoteCommand));

            if (exist == true)
            {
                var rcmdInfo = (from RemoteCommandInfo tempRemoteCommandInfo in this.RemoteCommandItems
                                where tempRemoteCommandInfo.RemoteCommand == remoteCommand
                                select tempRemoteCommandInfo).FirstOrDefault();

                if (rcmdInfo != null)
                {
                    result = this.RemoteCommandItems.Remove(rcmdInfo);
                }
            }
            else
            {
                var rcmdInfo = (from EnhancedRemoteCommandInfo tempRemoteCommandInfo in this.EnhancedRemoteCommandItems
                                where tempRemoteCommandInfo.RemoteCommand == remoteCommand
                                select tempRemoteCommandInfo).FirstOrDefault();

                if (rcmdInfo != null)
                {
                    result = this.EnhancedRemoteCommandItems.Remove(rcmdInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Enhanced Remote Command 정보를 가져옵니다.
        /// </summary>
        /// <param name="rcmd">가져올 Remote Command입니다.</param>
        /// <returns>Enhanced Remote Command 정보입니다.(단, 없을 경우 null)</returns>
        public EnhancedRemoteCommandInfo GetEnhancedRemoteCommand(string rcmd)
        {
            return this.EnhancedRemoteCommandItems.FirstOrDefault(t => t.RemoteCommand == rcmd);
        }
    }

    /// <summary>
    /// Remote Command Parameter Result 정보입니다.
    /// </summary>
    public class RemoteCommandParameterResult
    {
        private readonly string _cpName;
        private readonly int _parameterAck;

        /// <summary>
        /// Command Parameter Name을 가져옵니다.
        /// </summary>
        public string CPName
        {
            get { return this._cpName; }
        }

        /// <summary>
        /// Command Parameter Ack를 가져옵니다.
        /// </summary>
        public int ParameterAck
        {
            get { return this._parameterAck; }
        }

        /// <summary>
        /// Command Parameter Ack를 가져오거나 설정합니다.
        /// </summary>
        public List<RemoteCommandParameterResult> ParameterListAck { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="cpName">Command Parameter Name입니다.</param>
        public RemoteCommandParameterResult(string cpName)
        {
            this._cpName = cpName;

            this.ParameterListAck = new List<RemoteCommandParameterResult>();
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="cpName">Command Parameter Name입니다.</param>
        /// <param name="parameterAck">Command Parameter Ack입니다.</param>
        public RemoteCommandParameterResult(string cpName, int parameterAck)
        {
            this._cpName = cpName;
            this._parameterAck = parameterAck;

            this.ParameterListAck = new List<RemoteCommandParameterResult>();
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="parameterAck">Command Parameter Ack입니다.</param>
        public RemoteCommandParameterResult(int parameterAck)
        {
            this._cpName = string.Empty;
            this._parameterAck = parameterAck;

            this.ParameterListAck = new List<RemoteCommandParameterResult>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("CP Name={0}, Ack={1}", this._cpName, this._parameterAck);
        }
    }

    /// <summary>
    /// Remote Command Result 정보입니다.
    /// </summary>
    public class RemoteCommandResult
    {
        /// <summary>
        /// Remote Command Ack를 가져오거나 설정합니다.
        /// </summary>
        public int HostCommandAck { get; set; }

        /// <summary>
        /// Remote Command Parameter Result 정보를 가져오거나 설정합니다.
        /// </summary>
        public List<RemoteCommandParameterResult> Items { get; set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public RemoteCommandResult()
        {
            this.Items = new List<RemoteCommandParameterResult>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Host Command Ack={0}, Item Count={1}", this.HostCommandAck, this.Items.Count);
        }

        /// <summary>
        /// Remote Command Parameter Result 정보를 추가합니다.
        /// </summary>
        /// <param name="cpName">Command Parameter Name입니다.</param>
        /// <param name="parameterAck">Command Parameter Ack입니다.</param>
        public void AddParameterResult(string cpName, int parameterAck)
        {
            this.Items.Add(new RemoteCommandParameterResult(cpName, parameterAck));
        }
    }
}