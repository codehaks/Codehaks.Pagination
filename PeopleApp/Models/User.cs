namespace PeopleApp.Data
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public int Number { get; set; }

        [Required]
        [StringLength(6)]
        public string Gender { get; set; }

        [Required]
        [StringLength(25)]
        public string Nameset { get; set; }

        [Required]
        [StringLength(6)]
        public string Title { get; set; }

        [Required]
        [StringLength(20)]
        public string Givenname { get; set; }

        [Required]
        [StringLength(1)]
        public string Middleinitial { get; set; }

        [Required]
        [StringLength(23)]
        public string Surname { get; set; }

        [Required]
        [StringLength(100)]
        public string Streetaddress { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(22)]
        public string State { get; set; }

        [Required]
        [StringLength(100)]
        public string Statefull { get; set; }

        [Required]
        [StringLength(15)]
        public string Zipcode { get; set; }

        [Required]
        [StringLength(2)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string Countryfull { get; set; }

        [Required]
        [StringLength(100)]
        public string Emailaddress { get; set; }

        [Required]
        [StringLength(25)]
        public string Username { get; set; }

        [Required]
        [StringLength(25)]
        public string Password { get; set; }

        [Required]
        [StringLength(255)]
        public string Browseruseragent { get; set; }

        [Required]
        [StringLength(25)]
        public string Telephonenumber { get; set; }

        public int Telephonecountrycode { get; set; }

        [Required]
        [StringLength(20)]
        public string Maidenname { get; set; }

        [Required]
        [StringLength(10)]
        public string Birthday { get; set; }

        public int Age { get; set; }

        [Required]
        [StringLength(11)]
        public string Tropicalzodiac { get; set; }

        [Required]
        [StringLength(10)]
        public string Cctype { get; set; }

        [Required]
        [StringLength(16)]
        public string Ccnumber { get; set; }

        [Required]
        [StringLength(3)]
        public string Cvv2 { get; set; }

        [Required]
        [StringLength(10)]
        public string Ccexpires { get; set; }

        [Required]
        [StringLength(20)]
        public string Nationalid { get; set; }

        [Required]
        [StringLength(24)]
        public string Upstracking { get; set; }

        [Required]
        [StringLength(10)]
        public string Westernunionmtcn { get; set; }

        [Required]
        [StringLength(8)]
        public string Moneygrammtcn { get; set; }

        [Required]
        [StringLength(6)]
        public string Color { get; set; }

        [Required]
        [StringLength(70)]
        public string Occupation { get; set; }

        [Required]
        [StringLength(70)]
        public string Company { get; set; }

        [Required]
        [StringLength(255)]
        public string Vehicle { get; set; }

        [Required]
        [StringLength(70)]
        public string Domain { get; set; }

        [Required]
        [StringLength(3)]
        public string Bloodtype { get; set; }

        [Required]
        [StringLength(5)]
        public string Pounds { get; set; }

        [Required]
        [StringLength(5)]
        public string Kilograms { get; set; }

        [Required]
        [StringLength(6)]
        public string Feetinches { get; set; }

        [Required]
        [StringLength(3)]
        public string Centimeters { get; set; }

        [Required]
        [StringLength(36)]
        public string Guid { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}