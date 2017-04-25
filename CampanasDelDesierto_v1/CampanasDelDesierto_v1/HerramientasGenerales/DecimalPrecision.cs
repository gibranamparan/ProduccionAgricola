﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.HerramientasGenerales
{
    /// <summary>
    /// The Precision class allows us to decorate our Entity Models with a Precision attribute 
    /// to specify decimal precision values for the database column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DecimalPrecision : Attribute
    {

        /// <summary>
        /// The total number of digits to store, including decimals
        /// </summary>
        public byte precision { get; set; }
        /// <summary>
        /// The number of digits from the precision to be used for decimals
        /// </summary>
        public byte scale { get; set; }

        /// <summary>
        /// Define the precision and scale of a decimal data type
        /// </summary>
        /// <param name="precision">The total number of digits to store, including decimals</param>
        /// <param name="scale">The number of digits from the precision to be used for decimals</param>
        public DecimalPrecision(byte precision, byte scale)
        {
            this.precision = precision;
            this.scale = scale;
        }

        /// <summary>
        /// Apply the precision to our data model for any property using this annotation
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties().Where(x => x.GetCustomAttributes(false).OfType<DecimalPrecision>().Any())
                .Configure(c => c.HasPrecision(c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecision>().First()
                    .precision, c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecision>().First().scale));
        }
    }
}