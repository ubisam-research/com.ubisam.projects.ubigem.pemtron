using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UbiCom.Net.Structure;
using UbiGEM.Net.Structure;

namespace UbiGEM.Net.Simulator.Host
{
    public class RecipeManager
    {
        #region MemberVariable
        private readonly string _ppid;
        #endregion
        #region Property
        public string RecipeDirectory { get; set; }
        private string RecipeFilePath
        {
            get
            {
                string result;

                result = null;

                if (string.IsNullOrEmpty(this.RecipeDirectory) == false)
                {
                    result = string.Format(@"{0}\{1}.rcp", this.RecipeDirectory, this._ppid);
                }

                return result;
            }
        }
        #endregion

        #region Constructor
        public RecipeManager(string ppid)
        {
            this._ppid = ppid;
        }
        #endregion

        #region Load
        public List<FmtPPCCodeInfo> Load(out string errorText)
        {
            List<FmtPPCCodeInfo> result;
            FmtPPCCodeInfo ccodeInfo;
            FmtPPItem ppItem;
            XElement root;

            errorText = string.Empty;
            result = null;
            ccodeInfo = null;

            try
            {
                result = new List<FmtPPCCodeInfo>();

                if (System.IO.File.Exists(this.RecipeFilePath) == true)
                {
                    root = XElement.Load(this.RecipeFilePath);

                    if (root.Name == "Recipe")
                    {
                        if (root.Element("CCodeInfos") != null)
                        {
                            foreach(XElement ccodeElement in root.Element("CCodeInfos").Elements("CCodeInfo"))
                            {
                                if (ccodeElement.Attribute("CommandCode") != null)
                                {
                                    ccodeInfo = new FmtPPCCodeInfo
                                    {
                                        CommandCode = ccodeElement.Attribute("CommandCode").Value
                                    };
                                    result.Add(ccodeInfo);

                                }

                                if (ccodeInfo != null && ccodeElement.Element("PPItems") != null)
                                {
                                    foreach (XElement ppItemElement in ccodeElement.Element("PPItems").Elements("PPItem"))
                                    {
                                        if (ppItemElement.Attribute("PPName") != null && ppItemElement.Attribute("Format") != null)
                                        {
                                            if (Enum.TryParse(ppItemElement.Attribute("Format").Value, out SECSItemFormat format) == true && format != SECSItemFormat.L)
                                            {
                                                ppItem = new FmtPPItem
                                                {
                                                    PPName = ppItemElement.Attribute("PPName").Value,
                                                    Format = format,
                                                    PPValue = string.Empty
                                                };

                                                if (ppItemElement.Attribute("PPValue") != null)
                                                {
                                                    ppItem.PPValue = ppItemElement.Attribute("PPValue").Value;
                                                }

                                                ccodeInfo.Items.Add(ppItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorText = string.Format(" can not find file : {0}", this.RecipeFilePath);
                }
            }
            catch (Exception e)
            {
                errorText = e.Message;
            }

            return result;
        }
        #endregion

        #region Save
        public void Save(bool isLoaded, List<FmtPPCCodeInfo> codeCollection, out string errorText)
        {
            XElement root;
            XElement subElement;
            XElement subElement2;
            XElement codeInfoElement;

            errorText = string.Empty;

            if (codeCollection != null && isLoaded == true)
            {
                try
                {
                    root = new XElement("Recipe");

                    subElement = new XElement("CCodeInfos");
                    root.Add(subElement);

                    foreach (var codeInfo in codeCollection)
                    {
                        codeInfoElement = new XElement("CCodeInfo", 
                                                    new XAttribute("CommandCode", codeInfo.CommandCode));
                        subElement.Add(codeInfoElement);

                        subElement2 = new XElement("PPItems");
                        codeInfoElement.Add(subElement2);
                        foreach (var pp in codeInfo.Items)
                        {
                            subElement2.Add(new XElement("PPItem", 
                                                    new XAttribute("PPName", pp.PPName),
                                                    new XAttribute("Format", pp.Format),
                                                    new XAttribute("PPValue", pp.PPValue)
                            ));
                        }
                    }
                    root.Save(this.RecipeFilePath);
                }
                catch (Exception e)
                {
                    errorText = e.Message;
                }
            }
        }
        #endregion
    }
}