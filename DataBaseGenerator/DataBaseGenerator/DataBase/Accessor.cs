using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseGenerator.Readers;

namespace DataBaseGenerator.DataBase
{
    public static class DataWriter
    {
        static SQLiteConnection Connection = null;

        public static void Write(Data data)
        {
            Common.Log("Starting writing data to db...");
            System.IO.File.Delete("tickets.db");
            Connection = new SQLiteConnection("Data Source=tickets.db;Version=3;");
            Connection.Open();

            CreateSchema();
            InsertData(data);

            Connection.Close();
            Common.Log("Data successfully written to db.");
        }

        static void CreateSchema()
        {
            ExecuteCommand("CREATE TABLE Tickets (id integer primary key, num integer)");
            ExecuteCommand("CREATE TABLE Questions (id integer primary key, num integer, question text, image blob, ticket_id integer)");
            ExecuteCommand("CREATE TABLE Answers (id integer primary key, answer text, is_right integer, question_id integer)");
            ExecuteCommand("CREATE TABLE Chapters (id integer primary key, name text, content text)");
            ExecuteCommand("CREATE TABLE Marks (id integer primary key, num text, description text, image blob)");
            ExecuteCommand("CREATE TABLE Signs (id integer primary key, num text, description text, image blob)");
        }

        static void InsertData(Data data)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                using (var ticketCommand = Connection.CreateCommand())
                {
                    ticketCommand.CommandText = "INSERT INTO Tickets (num) Values (?)";
                    ticketCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.Int32 });
                    foreach (var ticket in data.Tickets)
                    {
                        ticketCommand.Parameters["@num"].Value = ticket.Num;
                        ticketCommand.ExecuteNonQuery();

                        using (var questionCommand = Connection.CreateCommand())
                        {
                            questionCommand.CommandText = "INSERT INTO Questions (num, question, image, ticket_id) Values (?,?,?,?)";
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.Int32 });
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@questions", DbType = System.Data.DbType.String });
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@image", DbType = System.Data.DbType.Binary });
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@ticket_id", DbType = System.Data.DbType.Int32, Value = ticket.Num });

                            using (var answerCommand = Connection.CreateCommand())
                            {
                                answerCommand.CommandText = "INSERT INTO Answers (answer, is_right, question_id) Values (?,?,?)";
                                answerCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@answer", DbType = System.Data.DbType.String });
                                answerCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@is_right", DbType = System.Data.DbType.Int32 });
                                answerCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@question_id", DbType = System.Data.DbType.Int32 });

                                foreach (var question in ticket.Questions)
                                {
                                    questionCommand.Parameters["@num"].Value = question.Num;
                                    questionCommand.Parameters["@questions"].Value = question.Text;
                                    questionCommand.Parameters["@image"].Value = question.Image;
                                    if (question.Image != null)
                                    {
                                        questionCommand.Parameters["@image"].Size = question.Image.Length;
                                    }
                                    questionCommand.ExecuteNonQuery();

                                    answerCommand.Parameters["@question_id"].Value = (ticket.Num - 1) * 20 + question.Num;
                                    foreach (var answer in question.Answers)
                                    {
                                        answerCommand.Parameters["@answer"].Value = answer.Text;
                                        answerCommand.Parameters["@is_right"].Value = answer.IsRight;
                                        answerCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                    }
                }

                using (var chapterCommand = Connection.CreateCommand())
                {
                    chapterCommand.CommandText = "INSERT INTO Chapters (name, content) Values (?,?)";
                    chapterCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@name", DbType = System.Data.DbType.String });
                    chapterCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@content", DbType = System.Data.DbType.String });
                    foreach (var chapter in data.Chapters)
                    {
                        chapterCommand.Parameters["@name"].Value = chapter.Name;
                        chapterCommand.Parameters["@content"].Value = chapter.Content;
                        chapterCommand.ExecuteNonQuery();
                    }
                }

                using (var markCommand = Connection.CreateCommand())
                {
                    markCommand.CommandText = "INSERT INTO Marks (num, description, image) Values (?,?,?)";
                    markCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.String });
                    markCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@description", DbType = System.Data.DbType.String });
                    markCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@image", DbType = System.Data.DbType.Binary });
                    foreach (var mark in data.Marks)
                    {
                        markCommand.Parameters["@num"].Value = mark.Num;
                        markCommand.Parameters["@description"].Value = mark.Description;
                        markCommand.Parameters["@image"].Value = mark.Image;
                        markCommand.Parameters["@image"].Size = mark.Image.Length;
                        markCommand.ExecuteNonQuery();
                    }
                }

                using (var signCommand = Connection.CreateCommand())
                {
                    signCommand.CommandText = "INSERT INTO Signs (num, description, image) Values (?,?,?)";
                    signCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.String });
                    signCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@description", DbType = System.Data.DbType.String });
                    signCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@image", DbType = System.Data.DbType.Binary });
                    foreach (var sign in data.Signs)
                    {
                        signCommand.Parameters["@num"].Value = sign.Num;
                        signCommand.Parameters["@description"].Value = sign.Description;
                        signCommand.Parameters["@image"].Value = sign.Image;
                        signCommand.Parameters["@image"].Size = sign.Image.Length;
                        signCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }

        }

        static void ExecuteCommand(String text)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = text;
                command.ExecuteNonQuery();
            }
        }
    }
}
