﻿namespace SampleProject.Domain.Queries.ViewModel
{
    public record OrderitemViewModel
    {
        public string productname { get; init; }
        public int units { get; init; }
        public double unitprice { get; init; }
        public string pictureurl { get; init; }
    }

    public record OrderViewModel
    {
        public int ordernumber { get; init; }
        public DateTime date { get; init; }
        public string status { get; init; }
        public string description { get; init; }
        public string street { get; init; }
        public string city { get; init; }
        public string zipcode { get; init; }
        public string country { get; init; }
        public List<OrderitemViewModel> orderitems { get; set; }
        public decimal total { get; set; }
    }

    public record OrderSummary
    {
        public int ordernumber { get; init; }
        public DateTime date { get; init; }
        public string status { get; init; }
        public double total { get; init; }
    }

    public record CardType
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}
