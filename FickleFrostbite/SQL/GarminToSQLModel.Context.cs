﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FickleFrostbite.SQL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GarminToSqlEntities : DbContext
    {
        public GarminToSqlEntities() : base("name=GarminToSqlEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Lap> Laps { get; set; }
        public virtual DbSet<Trackpoint> Trackpoints { get; set; }
        public virtual DbSet<DatabaseVersion> DatabaseVersions { get; set; }
        public virtual DbSet<DatabaseVersion_Log> DatabaseVersion_Log { get; set; }
    }
}
