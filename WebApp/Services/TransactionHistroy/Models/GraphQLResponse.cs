using System;

namespace VFL.Renderer.Services.TransactionHistroy.Models
{
    public class GraphQLResponse
    {
        public Allplanactivationsubscription[] allPlanActivationSubscriptions { get; set; }
        public Alltransactions allTransactions { get; set; }
        public Allsubscriptions allSubscriptions { get; set; }
        public Alldirecttopups allDirectTopUps { get; set; }
    }

    public class Allplanactivationsubscription
    {
        public DateTime serviceDate { get; set; }
        public string number { get; set; }
        public string plan_Name { get; set; }
        public float amount { get; set; }
        public string processTypeName { get; set; }
        public string completionStatusName { get; set; }
    }
    public class Allsubscriptions
    {
        public int totalCount { get; set; }
        public Edge[] edges { get; set; }
        public Pageinfo pageInfo { get; set; }
    }


    public class Alltransactions
    {
        public int totalCount { get; set; }
        public Edge[] edges { get; set; }
        public Pageinfo pageInfo { get; set; }
    }

    public class Pageinfo
    {
        public string endCursor { get; set; }
        public bool hasNextPage { get; set; }
    }

    public class Edge
    {
        public string cursor { get; set; }
        public Node node { get; set; }
    }

    public class Node
    {
        public DateTime serviceDate { get; set; }
        public string number { get; set; }
        public string reference { get; set; }
        public float amount { get; set; }
        public string paymentMethodName { get; set; }
        public string completionStatusName { get; set; }
        public object purchased_Plan_Name { get; set; }
        public string plan_Name { get; set; }
        public string processTypeName { get; set; }
        public string pin { get; set; }        
        public string gifterNumber { get; set; }        
    }

    public class Alldirecttopups
    {
        public int totalCount { get; set; }
        public Edge[] edges { get; set; }
        public Pageinfo pageInfo { get; set; }
    }
}
