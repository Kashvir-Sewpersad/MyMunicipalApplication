
/*------------------ start of file ------------*/ 
using Programming_7312_Part_1.Models;
using System.Collections.Generic;

namespace Programming_7312_Part_1.Services
{
    public class IssueStorage
    {
        private int _nextId = 1;

        public LinkedList<Issue> ReportedIssues { get; } = new LinkedList<Issue>(); // linked list for the reported issues

        public void AddIssue(Issue issue)
        {
            issue.Id = _nextId++;
            ReportedIssues.AddLast(issue);
        }
    }
}
//*------ end of file --------------//