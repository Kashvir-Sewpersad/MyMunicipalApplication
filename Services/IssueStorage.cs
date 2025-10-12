
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

        public Issue GetIssueById(int issueId)
        {
            return ReportedIssues.FirstOrDefault(i => i.Id == issueId);
        }

        public bool UpvoteIssue(int issueId)
        {
            var issue = GetIssueById(issueId);
            if (issue != null)
            {
                issue.Upvotes++;
                return true;
            }
            return false;
        }
    }
}
//*------ end of file --------------//