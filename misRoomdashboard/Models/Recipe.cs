//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rooms.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Recipe
    {
        public int ID { get; set; }
        public string recipe_name { get; set; }
        public string image { get; set; }
        public string desc { get; set; }
        public string number_servings { get; set; }
        public string preparation_time { get; set; }
        public string preparation_size { get; set; }
        public string yeild { get; set; }
        public string prepared_by { get; set; }
        public Nullable<int> userlevel { get; set; }
        public string time { get; set; }
        public Nullable<byte> status { get; set; }
        public Nullable<int> trail { get; set; }
        public Nullable<int> cal { get; set; }
        public Nullable<int> likes { get; set; }
        public Nullable<int> views { get; set; }
        public Nullable<int> category { get; set; }
    }
}