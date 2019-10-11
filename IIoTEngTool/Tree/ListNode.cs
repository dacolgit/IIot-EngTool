
using System.Collections.Generic;

namespace IIoTEngTool.tree
{
    public class ListNode
    {
        public int idNum { get; set; }
        public string id { get; set; }
        public string nodeClass { get; set; }
        public string accessLevel { get; set; }
        public string executable { get; set; }
        public string eventNotifier { get; set; }
        public string nextParentId { get; set; }
        public string parentName { get; set; }
        public bool children { get; set; }
        public string ImageUrl { get; set; }
        public string nodeName { get; set; }

        public List<string> parentIdList;

        public ListNode()
        {
            parentIdList = new List<string>();
        }
    }
}
