using System;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VFL.Renderer.Services.TransactionHistroy.Models
{
    public class PlanActivationSubscription
    {
        public string number { get; set; } = string.Empty;
        public string plan_Name { get; set; } = string.Empty;
        public string serviceDate { get; set; } = string.Empty;  // Or DateTime? if parsing needed
        public decimal amount { get; set; }  // Adjust type (e.g., double) based on API
        public string processTypeName { get; set; } = string.Empty;
        public string completionStatusName { get; set; } = string.Empty;
    }

    public class GraphQLRequest
    {
        public string query { get; set; }
        public GraphQLVariables Variables { get; set; }
    }
    public class GraphQLVariables
    {
        public int first { get; set; }
        public string after { get; set; }
        public Where where { get; set; }
    }

    public class Where
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Or[]? or { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CompletionStatusName? completionStatusName { get; set; }
        public Servicetypename serviceTypeName { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public Number? number { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public And[]? and { get; set; }
    }

    public class Or
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Number number { get; set; }
    }
    public class CompletionStatusName
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? eq { get; set; }
    }
    public class Servicetypename
    {
        public string eq { get; set; }
    }
    public class Number
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? eq { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? contains { get; set; }
    }
    //public class FromDate
    //{
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public string? eq { get; set; }
    //}

    //public class ToDate
    //{
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public string? eq { get; set; }
    //}
    //public class TransactionHistoryFilters
    //{
    //    public string serviceTypeName { get; set; }
    //    public string completionStatusName { get; set; }
    //    public string number { get; set; }
    //    public string FromDate { get; set; }
    //    public string ToDate { get; set; }
    //    public GraphQLRequest gqlRequest { get; set; }
    //}

    public class And
    {
        public Servicetypename serviceTypeName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CompletionStatusName? completionStatusName { get; set; }
        public And1[] and { get; set; }
    }
    public class And1
    {
        public Servicedate serviceDate { get; set; }
    }

    public class Servicedate
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? gte { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? lte { get; set; }
    }





}
