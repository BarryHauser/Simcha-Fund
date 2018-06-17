using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int TotalPll { get; set; }
        public int Contributers { get; set; }
        public decimal Money { get; set; }
    }
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AlwaysIncluded { get; set; }
        public string CellPhone { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class Contribution
    {
        public int PersonId { get; set; }
        public decimal Amount { get; set; }
    }
    public class IncludedContribution
    {
        public bool Included { get; set; }
        public Contribution Contribution { get; set; }
    }
    public class PersonWithBalance
    {
        public Person Person { get; set; }
        public decimal Balance { get; set; }
    }
    public class ContributersWithPast
    {
        public PersonWithBalance PersonWithBalance { get; set; }
        public decimal? Amount { get; set; }
    }
    public class Deposit
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
    public class Trancaction
    {
        public string Action { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
    public class SimchaDb
    {
        private string _connectionString;
        public SimchaDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void NewSimcha(Simcha simcha)
        {
            using (var connection = new SqlConnection(_connectionString))
                using(var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Simchas (Name,Date) VALUES (@Name,@Date)";
                cmd.Parameters.AddWithValue("@Name", simcha.Name);
                cmd.Parameters.AddWithValue("@Date", simcha.Date);
                connection.Open();
                cmd.ExecuteNonQuery();
                
            }
        }

        public IEnumerable<Simcha> GetAllSimchas()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "GetSimchasWithCounts";
                cmd.CommandType = CommandType.StoredProcedure;
                var simchas = new List<Simcha>();
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    
                    simchas.Add(new Simcha
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Date = (DateTime)reader["Date"],
                        TotalPll = (int)reader["TotalContributors"],
                        Contributers = (int)reader["Contributors"],
                        Money = DecimalNullChecker(reader["TotalContribution"])
                    });
                }
                
                return simchas;
            }
        }

        public IEnumerable<Contribution> GetContributionsBySimcha(int simchaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributions WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                var contributions = new List<Contribution>();
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    contributions.Add(new Contribution
                    {
                        PersonId = (int)reader["ContributorId"],
                        Amount = (decimal)reader["Amount"]
                    });
                }
                return contributions.Count > 0 ? contributions : null;
            }
        }

        public IEnumerable<PersonWithBalance> GetAllContributers()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributors";
                var Contributors = new List<PersonWithBalance>();
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int Id = (int)reader["Id"];
                    string FirstName = (string)reader["FirstName"];
                    string LastName = (string)reader["LastName"];
                    string CellPhone = (string)reader["CellPhone"];
                    bool AlwaysIncluded = (bool)reader["AlwaysIncluded"];
                    decimal Balance = GetBalanceOfAPerson(Id);
                    DateTime date = (DateTime)reader["DateCreated"];
                    Contributors.Add(new PersonWithBalance
                    {
                        Person = (new Person {
                            Id = Id,
                            FirstName = FirstName ,
                            LastName = LastName,
                            CellPhone = CellPhone,
                            AlwaysIncluded = AlwaysIncluded,
                            DateCreated = date
                            
                        }),
                        Balance = Balance
                    });
                }
                return Contributors;
            }
        }

        public Simcha GetSimchaById(int simchaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Simchas WHERE Id = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Simcha{
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                }
                return null;
            }
        }

        public void NewContributiosToASimcha (List<Contribution> contributions, int simchaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Contributions (SimchaId,ContributorId,Amount) VALUES (@SimchaId,@Id,@Amount)";
                connection.Open();
                foreach(Contribution c in contributions)
                {
                    cmd.Parameters.AddWithValue("@SimchaId", simchaId);
                    cmd.Parameters.AddWithValue("@Id", c.PersonId);
                    cmd.Parameters.AddWithValue("@Amount", c.Amount);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
        }

        public void ClearContributionsToASimcha(int simchaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE Contributions WHERE SimchaId = @SimchaId";
                cmd.Parameters.AddWithValue("@SimchaId", simchaId);
                connection.Open();
                cmd.ExecuteNonQuery();

            }
        }

        public decimal GetTotalBalance()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT SUM(Amount) AS Contributions, (SELECT SUM(Amount) FROM Deposits) AS Deposits  FROM Contributions";
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return (decimal)reader["Deposits"] - (decimal)reader["Contributions"];
                }
                return 0;
            }
        }

        public void NewContributer (Person person, decimal deposit)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Contributors (FirstName,LastName,CellPhone,AlwaysIncluded,DateCreated) VALUES (@FirstName,@LastName,@CellPhone,@AlwaysIncluded,@DateCreated)
                                    DECLARE @id INT =  CAST(SCOPE_IDENTITY() AS INT) 
                                    INSERT INTO Deposits (Amount,Date,ContributorId) VALUES (@Amount,@DateCreated,@id) ";
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@CellPhone", person.CellPhone);
                cmd.Parameters.AddWithValue("@AlwaysIncluded", person.AlwaysIncluded);
                cmd.Parameters.AddWithValue("@DateCreated", person.DateCreated);
                cmd.Parameters.AddWithValue("@Amount", deposit);
                connection.Open();
                cmd.ExecuteNonQuery();

            }
        }

        public void EditContributer(Person person)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Contributors
                                    SET FirstName = @FirstName, LastName = @LastName, CellPhone = @CellPhone, AlwaysIncluded = @AlwaysIncluded, DateCreated = @DateCreated,
                                    WHERE Id = @id;";
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@CellPhone", person.CellPhone);
                cmd.Parameters.AddWithValue("@AlwaysIncluded", person.AlwaysIncluded);
                cmd.Parameters.AddWithValue("@DateCreated", person.DateCreated);
                cmd.Parameters.AddWithValue("@id", person.Id);
                connection.Open();
                cmd.ExecuteNonQuery();

            }
        }

        public void Deposit (Deposit deposit, int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Deposits (Amount,Date,ContributorId) VALUES (@Amount,@Date,@id)";
                cmd.Parameters.AddWithValue("@Amount", deposit.Amount);
                cmd.Parameters.AddWithValue("@Date", deposit.Date);
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();

            }
        }

        public IEnumerable<Trancaction> GetTrancactionsById (int id)
        {
            var transctions = new List<Trancaction>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT C.Amount,S.Name,S.Date FROM Contributions C
                                    JOIN Simchas S
                                    ON C.SimchaId = S.Id
                                    WHERE C.ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    transctions.Add(new Trancaction
                    {
                        Action = $"Contribution to the {(String)reader["Name"]} simcha",
                        Amount = (decimal)reader["Amount"],
                        Date = (DateTime)reader["Date"]
                    });
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Deposits WHERE ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    transctions.Add(new Trancaction
                    {
                        Action = "deposit",
                        Amount = (decimal)reader["Amount"],
                        Date = (DateTime)reader["Date"]
                    });
                }
            }
            return transctions.OrderBy(t => t.Date);

        }

        public string GetNameById (int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributors WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);

                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return $"{(string)reader["FirstName"]} {(string)reader["LastName"]}";
                }
                return null;
            }
        }

        public decimal GetBalanceOfAPerson(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT SUM(Amount) AS Contributions, (SELECT SUM(Amount) FROM Deposits WHERE ContributorId = @id) AS Deposits  FROM Contributions WHERE ContributorId = @id ";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return (decimal)reader["Deposits"] - DecimalNullChecker(reader["Contributions"]);
                }
                return 0;
            }
        }
        
        public int IntNullChecker (object obj)
        {
            return obj == DBNull.Value ? 0 : (int)obj;
        }
        public decimal DecimalNullChecker(object obj)
        {
            return obj == DBNull.Value ? 0 : (decimal)obj;
        }
    }
}
