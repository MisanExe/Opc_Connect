
using Opc.UaFx;
using Opc.UaFx.Client;

namespace CodeSysAppNs
{

    public partial class CodeSysApp
    {
        /// <summary>
        /// CodeSys app constructor poiints to current running opcua client 
        /// </summary>
        /// <param name="cli"></param>
        ///
        public CodeSysApp(OpcClient cli)
        {
            this.client = cli;
            MachineStateList[1] = false;
            MachineStateList[2] = false;
            MachineStateList[3] = false;
            MachineStateList[4] = false;
            MachineStateList[5] = false;
            MachineStateList[6] = false;
            MachineStateList[7] = false;
            MachineStateList[8] = false;
            MachineStateList[9] = false;
            MachineStateList[10] = false;
        }
        /// <summary>
        /// CodeSys app constructor
        /// </summary>
        public CodeSysApp(){}

        /// <summary>
        /// Attributes
        /// </summary>
        
        private OpcNodeInfo? root = null;
        public List<OpcNodeInfo>? TagNodes = new List<OpcNodeInfo>();
        public Dictionary<string,OpcNodeInfo> ? TagTuple = new Dictionary<string, OpcNodeInfo>();
        public OpcClient? client;
        public Dictionary<int,bool> MachineStateList = new Dictionary<int, bool>();


        //Properties
        public bool IsConnected
        {
            get;
            private set;
        }
        public string? ProgramName
        {
            get;
            set;
        }

        public OpcNodeId? ProgramNodeId
        {
            get;
            private set;
        }

        public OpcNodeInfo? RootNodeInfo
        {
            set
            {
                root = value;
            }
        }

        //Methods
        public void Run()
        {
            //get Nodes
            if (this.root != null)
            {
                this.GetProgramNode(this.root);
                this.PrintTags();
                this.SetHandShake();
            }
            else
            {
                Console.WriteLine("omor cannot create client oh!");
            }
        }
        private static OpcNodeInfo? BrowseNode(OpcNodeInfo node, string objectName, int level = 0)
        {
            level++;

            foreach (var childNode in node.Children())
            {
                OpcText plc_name = new OpcText("PLC_PRG");

                if (childNode.DisplayName.Value == objectName)
                {
                    return childNode;
                }
                else
                {
                    OpcNodeInfo? foundNode = BrowseNode(childNode, objectName, level);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }
            return null;
        }


        public OpcNodeId GetProgramNode(OpcNodeInfo root_info)
        {
            //get root node

            var ProgramNodeInfo = BrowseNode(root_info, this.ProgramName);
            ProgramNodeId = ProgramNodeInfo?.NodeId;

            foreach (var child in ProgramNodeInfo.Children())
            {
                TagNodes.Add(child);
                TagTuple.Add(child.DisplayName.Value, child);
            }


            return ProgramNodeInfo.NodeId;
        }


        public void PrintTags()
        {
            if(TagNodes != null)
            {
                foreach (var tag in TagNodes)
                {
                    Console.WriteLine(tag.DisplayName);
                }
            }
            else
            {
                Console.WriteLine("Dictionary is empty!");
            }
           
        }

        private bool FindHandShake()
        {
            return TagTuple["UA_HandShake"] != null ? true : false;
        }

        private void SetHandShake()
        {
            if (FindHandShake())
            {
                this.client?.WriteNode( GetTagByName("UA_HandShake")?.NodeId, true);
                if (GetTagByName("UA_HandShake")?.NodeId == null)
                {
                    Console.WriteLine("Cannot find HandShake Tag");
                }
            }
            else
            {
                Console.WriteLine("Cannot find HandShake Tag");
            }
        }

        private OpcNodeInfo ? GetTagByName(string? TagName)
        {
            return TagTuple[TagName] ?? null;
        } 

        public bool SubscribeTo(string TagName, OpcDataChangeReceivedEventHandler Handler)
        {
            OpcSubscription? subscription = this.client?.SubscribeDataChange(GetTagByName(TagName)?.NodeId, Handler);
            return false;
        }

        public void ClearState()
        {
            if(this.MachineStateList != null)
            {
                for(int state = 0; state < this.MachineStateList.Count; state++)
                {
                    MachineStateList[state] = false;
                }
            }
           
        }

       


    }


}