using Atlantis.Data.Common.Base;
using Playland.Database.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Playland.Database
{
    public class DashboardFactory
    {
        public GenericResponse<Summary> GetSummaries(string startDate, string endDate, int typeId)
        {
            GenericResponse<Summary> genericResponse = new GenericResponse<Summary>();

            if (typeId == 3)
            {
                Summary summary = new Summary()
                {
                    CardPlays = new List<CardPlayCount>()
                };
                for (int i = 0; i < typeId; i++)
                {
                    GenericResponse<Summary> summaryResponse = GetSummaries(startDate, endDate, i);
                    if (summaryResponse.IsSucceed)
                    {
                        Summary tempTransac = summaryResponse.Result;
                        summary.TotalBonus += tempTransac.TotalBonus;
                        summary.TotalCashAmount += tempTransac.TotalCashAmount;
                        summary.TotalCreditCard += tempTransac.TotalCreditCard;
                        summary.TotalFreeGame += tempTransac.TotalFreeGame;
                        summary.TotalGameAmount += tempTransac.TotalGameAmount;
                        summary.TotalTransaction += tempTransac.TotalTransaction;
                        summary.TotalUploadQuantity += tempTransac.TotalUploadQuantity;
                        summary.TotalVisitor += tempTransac.TotalVisitor;

                        summary.CardPlays.AddRange(tempTransac.CardPlays);
                    }
                }

                genericResponse.IsSucceed = true;
                genericResponse.Result = summary;
                return genericResponse;
            }

            using (SQLDbFactory factory = new SQLDbFactory(typeId))
            {

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.AddParameters("StartDate", DateTime.ParseExact(startDate, "dd.MM.yyyy", CultureInfo.InvariantCulture));
                parameters.AddParameters("EndDate", DateTime.ParseExact(endDate, "dd.MM.yyyy", CultureInfo.InvariantCulture));

                DataSet dataSet = factory.GetDataSet("GetGameDaySummaries", parameters);
                List<DataRow> dataRows = dataSet.Tables[0].Rows.OfType<DataRow>().ToList();

                DataTable dtDet = dataSet.Tables[1];

                if (dataRows != null)
                {
                    genericResponse.IsSucceed = true;
                    genericResponse.Result = new Summary()
                    {
                        TotalTransaction = dataRows.Sum(f => f.Field<decimal>("TotalTransaction")),
                        TotalFreeGame = dataRows.Sum(f => f.Field<int>("FreeGameCount")),
                        TotalGameCount = dataRows.Sum(f => f.Field<int>("TotalGameCount")),
                        TotalGameAmount = dataRows.Sum(f => f.Field<decimal>("TotalGameAmount")),
                        TotalCashAmount = dataRows.Sum(f => f.Field<decimal>("TotalCash")),
                        TotalCreditCard = dataRows.Sum(f => f.Field<decimal>("TotalCreditCard")),
                        TotalBonus = dataRows.Sum(f => f.Field<decimal>("TotalBonus")),
                        TotalUploadQuantity = dataRows.Sum(f => f.Field<int>("TotalUploadQuantity")),
                        TotalVisitor = dataRows.Sum(f => f.Field<int>("TotalCustomerVisit")),
                    };

                    genericResponse.Result.CardPlays = new List<CardPlayCount>();

                    foreach (DataRow row in dtDet.Rows)
                    {
                        CardPlayCount pc = new CardPlayCount();
                        pc.CardNo = row["CardNo"] + "";
                        pc.PlayCount = Convert.ToInt32(row["PlayCount"]);

                        genericResponse.Result.CardPlays.Add(pc);

                    }
                }


            }

            return genericResponse;
        }


        public GenericResponse<List<CardUpload>> GetCardUploads(string startDate, string endDate, int typeId)
        {
            GenericResponse<List<CardUpload>> genericResponse = new GenericResponse<List<CardUpload>>();
            using (SQLDbFactory factory = new SQLDbFactory(typeId))
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.AddParameters("StartDate", DateTime.ParseExact(startDate, "dd.MM.yyyy", CultureInfo.InvariantCulture));
                parameters.AddParameters("EndDate", DateTime.ParseExact(endDate, "dd.MM.yyyy", CultureInfo.InvariantCulture));

                DataSet dataSet = factory.GetDataSet("WebGetCardUploads", parameters);
                DataTable dataTable = dataSet.Tables[0];
                DataRowCollection dataRows = dataTable.Rows;
                List<CardUpload> cardUploads = new List<CardUpload>();

                DataRowCollection detailDtRow = dataSet.Tables[1].Rows;

                foreach (DataRow item in dataRows)
                {
                    CardUpload upload = new CardUpload()
                    {
                        CardUID = item["CardUID"].ToString(),
                        Amount = decimal.Parse(item["UploadAmount"].ToString()),
                        Quantity = int.Parse(item["UploadQuantity"].ToString()),
                        IsNewCard = bool.Parse(item["IsNewCard"].ToString()),
                    };

                    List<DataRow> dataRowDetails = detailDtRow.OfType<DataRow>().Where(f => f.Field<string>("CardUID") == upload.CardUID).ToList();
                    if (dataRowDetails != null && dataRowDetails.Count > 0)
                    {
                        foreach (DataRow dtItem in dataRowDetails)
                        {
                            CardTransaction transaction = new CardTransaction()
                            {
                                Amount = decimal.Parse(dtItem["Amount"].ToString()),
                                CreationDate = DateTime.Parse(dtItem["CreatedDate"].ToString()).ToString("dd.MM.yyyy HH:mm:ss"),
                            };
                            upload.Transactions.Add(transaction);
                        }
                    }

                    cardUploads.Add(upload);
                }

                genericResponse.Result = cardUploads;
            }
            return genericResponse;
        }


        public GenericResponse<User> Authenticate(User user)
        {
            GenericResponse<User> genericResponse = new GenericResponse<User>();

            using (SQLDbFactory factory = new SQLDbFactory())
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.AddParameters("Username", user.Username);
                parameters.AddParameters("Password", user.Password);

                List<DataRow> dataRows = factory.GetDataRows("UserLogin", parameters);

                if (dataRows != null && dataRows.Count > 0)
                {
                    genericResponse.Result = new User()
                    {
                        UserId = int.Parse(dataRows[0]["UserId"].ToString()),
                        Username = dataRows[0]["Username"].ToString(),
                        Password = dataRows[0]["Password"].ToString(),
                    };
                    genericResponse.IsSucceed = true;
                }
                else
                {
                    genericResponse.Error = new Error()
                    {
                        ErrorMessage = AppResources.App_InvalidUser,
                    };
                    return genericResponse;
                }
            }

            return genericResponse;
        }
    }

    public static class DbExtensions
    {
        public static void AddParameters(this List<SqlParameter> parameters, string key, object value)
        {
            if (parameters == null)
            {
                parameters = new List<SqlParameter>();
            }

            parameters.Add(new SqlParameter()
            {
                ParameterName = key,
                Value = value,
            });
        }
    }
}
