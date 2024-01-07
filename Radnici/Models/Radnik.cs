using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Radnici.Models
{
    public class Radnik
    {
        

        public int Id { get; set; }

        [Display(Name = "Ime")]
        public string? ime { get; set; }

        [Display(Name = "Prezime")]
        public string? prezime { get; set; }

        [Display(Name = "Adresa")]
        public string? adresa { get; set; }

        [Display(Name = "Iznos neto plate")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal iznosNetoPlate { get; set; }

        [Display(Name = "Radna pozicija")]
        public string? radnaPozicija { get; set; }


    }

}

