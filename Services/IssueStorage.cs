
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ START OF FILE @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@//

using Programming_7312_Part_1.Models;
using Programming_7312_Part_1.Data;
using System.Collections.Generic;
using System.Linq;
using System;

// ========================== BST Node ============================== 
public class BSTNode<T>
{
    public T Data { get; set; }
    public BSTNode<T> Left { get; set; }
    public BSTNode<T> Right { get; set; }

    public BSTNode(T data)
    {
        Data = data;
    }
}

// ==============================  Binary Search Tree ================================= 
public class BinarySearchTree<T>
{
    public BSTNode<T> Root { get; private set; }
    private readonly IComparer<T> comparer;

    public BinarySearchTree(IComparer<T>? comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
    }

    public void Insert(T item)
    {
        Root = InsertRec(Root, new BSTNode<T>(item));
    }

    private BSTNode<T> InsertRec(BSTNode<T> root, BSTNode<T> newNode)
    {
        if (root == null)
        {
            root = newNode;
            return root;
        }
        if (comparer.Compare(newNode.Data, root.Data) < 0)
            root.Left = InsertRec(root.Left, newNode);
        else
            root.Right = InsertRec(root.Right, newNode);
        return root;
    }

    public List<T> InOrderTraversal()
    {
        var result = new List<T>();
        InOrderRec(Root, result);
        return result;
    }

    private void InOrderRec(BSTNode<T> root, List<T> result)
    {
        if (root != null)
        {
            InOrderRec(root.Left, result);
            result.Add(root.Data);
            InOrderRec(root.Right, result);
        }
    }
}

// ==========================  AVL Tree Node (simplified balancing) ================================= 
public class AVLNode<T>
{
    public T Data { get; set; }
    public AVLNode<T> Left { get; set; }
    public AVLNode<T> Right { get; set; }
    public int Height { get; set; } = 1;

    public AVLNode(T data)
    {
        Data = data;
    }
}

public class AVLTree<T>
{
    public AVLNode<T> Root { get; private set; }
    private readonly IComparer<T> comparer;

    public AVLTree(IComparer<T>? comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
    }

    public void Insert(T item)
    {
        Root = InsertRec(Root, new AVLNode<T>(item));
    }

    private AVLNode<T> InsertRec(AVLNode<T> node, AVLNode<T> newNode)
    {
        if (node == null) return newNode;

        if (comparer.Compare(newNode.Data, node.Data) < 0)
            node.Left = InsertRec(node.Left, newNode);
        else
            node.Right = InsertRec(node.Right, newNode);

        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

        // ------------------------ Balance factor
        int balance = GetBalance(node);

        // ---------------------------------------- Rotations (simplified)
        if (balance > 1 && comparer.Compare(newNode.Data, node.Left.Data) < 0)
            return RightRotate(node);
        if (balance < -1 && comparer.Compare(newNode.Data, node.Right.Data) > 0)
            return LeftRotate(node);
        if (balance > 1 && comparer.Compare(newNode.Data, node.Left.Data) > 0)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }
        if (balance < -1 && comparer.Compare(newNode.Data, node.Right.Data) < 0)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    private int GetHeight(AVLNode<T> node) => node?.Height ?? 0;
    private int GetBalance(AVLNode<T> node) => node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);

    private AVLNode<T> RightRotate(AVLNode<T> y)
    {
        AVLNode<T> x = y.Left;
        AVLNode<T> T2 = x.Right;
        x.Right = y;
        y.Left = T2;
        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));
        return x;
    }

    private AVLNode<T> LeftRotate(AVLNode<T> x)
    {
        AVLNode<T> y = x.Right;
        AVLNode<T> T2 = y.Left;
        y.Left = x;
        x.Right = T2;
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));
        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
        return y;
    }

    public List<T> InOrderTraversal()
    {
        var result = new List<T>();
        InOrderRec(Root, result);
        return result;
    }

    private void InOrderRec(AVLNode<T> root, List<T> result)
    {
        if (root != null)
        {
            InOrderRec(root.Left, result);
            result.Add(root.Data);
            InOrderRec(root.Right, result);
        }
    }
}

// ============================  Graph for dependencies ================================
public class ServiceRequestGraph
{
    private Dictionary<int, List<(int toId, int weight)>> adjacencyList = new Dictionary<int, List<(int, int)>>();

    public void AddDependency(int fromId, int toId, int weight = 1)
    {
        if (!adjacencyList.ContainsKey(fromId))
            adjacencyList[fromId] = new List<(int, int)>();
        if (!adjacencyList[fromId].Any(e => e.toId == toId))
            adjacencyList[fromId].Add((toId, weight));
    }

    public List<int> GetDependencies(int issueId)
    {
        return adjacencyList.GetValueOrDefault(issueId, new List<(int, int)>()).Select(e => e.Item1).ToList();
    }

    // =================================== BFS Traversal for dependency path =====================================
    public List<int> GetDependencyPath(int startId)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(startId);
        visited.Add(startId);
        var path = new List<int> { startId };

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            var deps = GetDependencies(current);
            foreach (int dep in deps)
            {
                if (!visited.Contains(dep))
                {
                    visited.Add(dep);
                    queue.Enqueue(dep);
                    path.Add(dep);
                }
            }
        }
        return path;
    }

    // =================================== Minimum Spanning Tree using Kruskal's Algorithm =====================================
    public List<(int from, int to, int weight)> GetMinimumSpanningTree()
    {
        var edges = new List<(int from, int to, int weight)>();
        foreach (var kvp in adjacencyList)
        {
            foreach (var edge in kvp.Value)
            {
                edges.Add((kvp.Key, edge.toId, edge.weight));
            }
        }

        // =================================Sort edges by weight================================
        edges.Sort((a, b) => a.weight.CompareTo(b.weight));

        var parent = new Dictionary<int, int>();
        var rank = new Dictionary<int, int>();

        // ===================================Initialize Union====================================
        foreach (var node in adjacencyList.Keys.Concat(adjacencyList.Values.SelectMany(l => l.Select(e => e.toId))))
        {
            if (!parent.ContainsKey(node))
            {
                parent[node] = node;
                rank[node] = 0;
            }
        }

        var mst = new List<(int from, int to, int weight)>();

        foreach (var edge in edges)
        {
            int rootFrom = Find(parent, edge.from);
            int rootTo = Find(parent, edge.to);

            if (rootFrom != rootTo)
            {
                mst.Add(edge);
                Union(parent, rank, rootFrom, rootTo);
            }
        }

        return mst;
    }

    private int Find(Dictionary<int, int> parent, int node)
    {
        if (parent[node] != node)
            parent[node] = Find(parent, parent[node]);
        return parent[node];
    }

    private void Union(Dictionary<int, int> parent, Dictionary<int, int> rank, int root1, int root2)
    {
        if (rank[root1] > rank[root2])
            parent[root2] = root1;
        else if (rank[root1] < rank[root2])
            parent[root1] = root2;
        else
        {
            parent[root2] = root1;
            rank[root1]++;
        }
    }
}

namespace Programming_7312_Part_1.Services
{
    public class IssueStorage
    {
        private readonly ApplicationDbContext _context;
        private int _nextId = 1;

        // ---------------------Keep in-memory structures for advanced operations ---------------------- 
        public BinarySearchTree<Issue> BstById { get; } = new BinarySearchTree<Issue>(Comparer<Issue>.Create((a, b) => a.Id.CompareTo(b.Id)));
        public AVLTree<Issue> AvlByDate { get; } = new AVLTree<Issue>(Comparer<Issue>.Create((a, b) => a.ReportedDate.CompareTo(b.ReportedDate)));
        public SortedSet<Issue> RedBlackByCategory { get; } = new SortedSet<Issue>(Comparer<Issue>.Create((a, b) => string.Compare(a.Category, b.Category)));
        public PriorityQueue<Issue, int> HeapByPriority = new PriorityQueue<Issue, int>(); // ---------------------> Min-heap by upvotes (negated for max)
        public ServiceRequestGraph Graph { get; } = new ServiceRequestGraph();

        public IssueStorage(ApplicationDbContext context)
        {
            _context = context;
            LoadIssuesFromDatabase();
        }
//========================== RETRIVE THE ISSUES FORM THE DATABASE =============================//
        private void LoadIssuesFromDatabase()
        {
            var issues = _context.Issues.ToList();
            foreach (var issue in issues)
            {
                // Update structures
                BstById.Insert(issue);
                AvlByDate.Insert(issue);
                RedBlackByCategory.Add(issue);
                HeapByPriority.Enqueue(issue, -issue.Upvotes); // Negate for max-heap simulation

                if (issue.Id >= _nextId) _nextId = issue.Id + 1;
            }
        }
//========================== ADD ISSUE =========================//
        public void AddIssue(Issue issue)
        {
            issue.Id = _nextId++;
            _context.Issues.Add(issue);
            _context.SaveChanges();

            // Update structures (use Id for BST, ReportedDate for AVL, Category for RedBlack, -Upvotes for heap max)
            BstById.Insert(issue);
            AvlByDate.Insert(issue);
            RedBlackByCategory.Add(issue);
            HeapByPriority.Enqueue(issue, -issue.Upvotes); // Negate for max-heap simulation

            // Sample dep (in real: from form)
            if (issue.Id > 1) Graph.AddDependency(issue.Id, issue.Id - 1);
        }

        public Issue GetIssueById(int issueId)
        {
            return _context.Issues.Find(issueId);
        }
//================================= UPVOTE SYSTEM BASED OFF THE ISSSSUE ID ========================// 
        public bool UpvoteIssue(int issueId)
        {
            var issue = _context.Issues.Find(issueId);
            if (issue != null)
            {
                issue.Upvotes++;
                _context.SaveChanges();
                // Update heap priority - rebuild the heap
                var tempHeap = new PriorityQueue<Issue, int>();
                foreach (var i in _context.Issues) tempHeap.Enqueue(i, -i.Upvotes);
                HeapByPriority = tempHeap;
                return true;
            }
            return false;
        }
//================================= DOWN VOTE SYSTEM BASED OFF THE ISSSSUE ID ========================// 
        public bool DownvoteIssue(int issueId)
        {
            var issue = _context.Issues.Find(issueId);
            if (issue != null)
            {
                issue.Downvotes++;
                _context.SaveChanges();
                // Update heap priority - rebuild the heap (using upvotes for priority)
                var tempHeap = new PriorityQueue<Issue, int>();
                foreach (var i in _context.Issues) tempHeap.Enqueue(i, -i.Upvotes);
                HeapByPriority = tempHeap;
                return true;
            }
            return false;
        }
//============================= GET THE USER ISSUES =========================================//
        public List<Issue> GetUserIssues(string userId)
        {
            return _context.Issues.Where(i => i.UserId == userId).OrderByDescending(i => i.Upvotes).ToList();
        }

        public List<Issue> GetAllIssues()
        {
            return _context.Issues.OrderByDescending(i => i.Upvotes).ToList();
        }
// ============================== APPROVE ISSUE AND SAVE THE CHANGE ==========================
        public bool ApproveIssue(int issueId, string comments = null)
        {
            var issue = _context.Issues.Find(issueId);
            if (issue != null)
            {
                issue.AdminResponse = "Approved"; // MESSAGE 
                issue.AdminComments = comments;
                issue.ResponseDate = DateTime.Now; // TIME STAMP 
                issue.Status = "Approved";
                _context.SaveChanges(); // SAVE 
                return true;
            }
            return false;
        }

        public bool RejectIssue(int issueId, string comments = null)
        {
            var issue = _context.Issues.Find(issueId);
            if (issue != null)
            {
                issue.AdminResponse = "Rejected";
                issue.AdminComments = comments;
                issue.ResponseDate = DateTime.Now;
                issue.Status = "Rejected";
                _context.SaveChanges();
                return true;
            }
            return false;
        }
//============================= DELETE THE ISSUE ==================================//
        public bool DeleteIssue(int issueId, string comments = null)
        {
            var issue = _context.Issues.Find(issueId);
            if (issue != null)
            {
                issue.AdminResponse = "Deleted";
                issue.AdminComments = comments;
                issue.ResponseDate = DateTime.Now;
                issue.Status = "Deleted";
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

// =============================== Extend Issue for comparers ================================== 
public static class IssueComparer
{
    public static int ByDate(Issue a, Issue b) => a.ReportedDate.CompareTo(b.ReportedDate);
    public static int ById(Issue a, Issue b) => a.Id.CompareTo(b.Id);
}

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  END  OF FILE @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@//