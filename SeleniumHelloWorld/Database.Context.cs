﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SeleniumHelloWorld
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TestBD1Entities : DbContext
    {
        public TestBD1Entities()
            : base("name=TestBD1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PeopleWorkEdu> PeopleWorkEdus { get; set; }
        public virtual DbSet<PeoplePlace> PeoplePlaces { get; set; }
        public virtual DbSet<FamilyRole> FamilyRoles { get; set; }
    }
}
