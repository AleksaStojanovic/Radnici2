using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Radnici.Data;
using Radnici.Models;
using System;
using System.Linq;

namespace MvcMovie.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new RadniciContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<RadniciContext>>()))
        {
            // Look for any movies.
            if (context.Radnik.Any())
            {
                return;   // DB has been seeded
            }
            context.Radnik.AddRange(
                new Radnik
                {
                    ime = "Stefan",
                    prezime = "Stefanovic",
                    adresa = "Ulica 1",
                    iznosNetoPlate =50000M,
                    radnaPozicija="traktor"
                    
                },
               new Radnik
               {
                   ime = "Jovan",
                   prezime = "Jovanovic",
                   adresa = "Ulica 2",
                   iznosNetoPlate = 60000M,
                   radnaPozicija = "kamion"

               },
               new Radnik
               {
                   ime = "Marko",
                   prezime = "Markovic",
                   adresa = "Ulica 3",
                   iznosNetoPlate = 70000M,
                   radnaPozicija = "autobus"

               },
               new Radnik
               {
                   ime = "Nikola",
                   prezime = "Nikolic",
                   adresa = "Ulica 4",
                   iznosNetoPlate = 80000M,
                   radnaPozicija = "kombi"

               }
            );
            context.SaveChanges();
        }
    }
}