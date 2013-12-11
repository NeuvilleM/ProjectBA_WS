using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Web;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace JSONapi.Models
{
    public class FestivalData
    {

        private ObservableCollection<StageGroup> _stages;

        public ObservableCollection<StageGroup> Stages
        {
            get { return this._stages; }
            set { _stages = value; }
        }
        public  ObservableCollection<StageGroup> GetFestivalData()
        {
            if (this.Stages != null && this._stages.Count != 0)
                return this._stages;
            ObservableCollection<StageGroup> stages = new ObservableCollection<StageGroup>();
            // inladen stage
            // https://blogs.oracle.com/MySqlOnWindows/entry/how_to_using_connector_net
            //		per stage de linups ophalen
            //			per linup de matchende band ophalen
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3306;Database=festivalapp;Uid=root;Pwd=root"))
            {
                connection.Open();
                // stage list
                MySqlCommand stagesCommand = new MySqlCommand("SELECT * FROM stages", connection);
                using (MySqlDataReader reader = stagesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StageGroup group = new StageGroup(reader["Id"].ToString(), reader["StageName"].ToString());
                        int i = -1;
                        if (Int32.TryParse(group.UniqueId, out i) && i > 0)
                        {
                            string sql = "SELECT * FROM lineup WHERE Stage = " + i.ToString();
                            MySqlConnection conlinup = new MySqlConnection("Server=localhost;Port=3306;Database=festivalapp;Uid=root;Pwd=root");
                            conlinup.Open();
                            MySqlCommand lineupCommand = new MySqlCommand(sql, conlinup);
                            using (MySqlDataReader readerlineup = lineupCommand.ExecuteReader())
                            {
                                ObservableCollection<lineupItem> lineups = new ObservableCollection<lineupItem>();
                                while (readerlineup.Read())
                                {
                                    ArtiestItem artiest = new ArtiestItem("-1", "", "ERROR", "ERROR artiest is null", "", "", new List<string>());
                                    string sqlartist = "SELECT * FROM artist WHERE Id=" + readerlineup["Artist"].ToString();
                                    MySqlConnection conartist = new MySqlConnection("Server=localhost;Port=3306;Database=festivalapp;Uid=root;Pwd=root");
                                    conartist.Open();
                                    MySqlCommand artistCommand = new MySqlCommand(sqlartist, conartist);
                                    using (MySqlDataReader readerartist = artistCommand.ExecuteReader())
                                    {
                                        while (readerartist.Read())
                                        {
                                            List<string> genres = new List<string>();
                                            string sqlGenres = "SELECT genres.GenreNaam FROM artist_genre INNER JOIN genres on artist_genre.GenreID=genres.Id WHERE ArtistID=" + readerlineup["Artist"].ToString();
                                            MySqlConnection congenres = new MySqlConnection("Server=localhost;Port=3306;Database=festivalapp;Uid=root;Pwd=root");
                                            congenres.Open();
                                            MySqlCommand genresCommand = new MySqlCommand(sqlGenres, congenres);
                                            using (MySqlDataReader readergenres = genresCommand.ExecuteReader())
                                            {
                                                while (readergenres.Read())
                                                {
                                                    genres.Add(readergenres["GenreNaam"].ToString());
                                                }
                                                readergenres.Close();
                                            }
                                            artiest = new ArtiestItem(readerartist["Id"].ToString(), readerartist["Picture"].ToString(), readerartist["Naam"].ToString(), readerartist["Description"].ToString(), readerartist["Twitter"].ToString(), readerartist["Facebook"].ToString(), genres);
                                            congenres.Close();
                                        }
                                        readerartist.Close();
                                        
                                    }
                                    conartist.Close();
                                    //if (artiest == null) artiest = new ArtiestItem("-1", "", "ERROR", "ERROR artiest is null", "", "", new List<string>());
                                    lineupItem lineup = new lineupItem(readerlineup["Id"].ToString(), group.UniqueId, readerlineup["DateOfPlay"].ToString(), readerlineup["Start"].ToString(), readerlineup["End"].ToString(), artiest);
                                    lineups.Add(lineup);

                                }
                                group.Items = lineups;
                                readerlineup.Close();
                                conlinup.Close();
                            }
                        }
                        stages.Add(group);

                    }
                    reader.Close();
                }
            }
            return stages;

        }
        public void GeefStages()
        {
            ObservableCollection<StageGroup> stages = this.Stages;
            ObservableCollection<StageGroup> stageTag = new ObservableCollection<StageGroup>();
            
        
        }
    }
    public class ArtiestItem
    {
        public ArtiestItem(String uniqueId, String Picture, String Name, String description, String twitter, String facebook, List<string> genres)
        {
            this.UniqueId = uniqueId;
            this.Picture = Picture;
            this.Name = Name;
            this.Description = description;
            this.Facebook = facebook;
            this.Twitter = twitter;
            this.Genres = genres;
        }

        public string UniqueId { get; private set; }
        public string Picture { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Facebook { get; private set; }
        public string Twitter { get; private set; }
        public List<string> Genres { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class StageGroup
    {
        public StageGroup(String uniqueId, String name)
        {
            this.UniqueId = uniqueId;
            this.Name = name;
            this.Items = new ObservableCollection<lineupItem>();
        }
        public string UniqueId { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<lineupItem> Items { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
    public class lineupItem
    {
        public lineupItem(String uniqueId, string stageId, string dateOfPlay, string start, string end, ArtiestItem artist)
        {
            this.artiest = artist;
            this.UniqueId = uniqueId;
            this.StageId = stageId;
            this.DateOfPlay = dateOfPlay;
            this.Start = start;
            this.End = end;

        }
        public ArtiestItem artiest { get; set; }
        public String StageId { get; private set; }
        public string DateOfPlay { get; private set; }
        public string Start { get; private set; }
        public string End { get; private set; }
        public string UniqueId { get; private set; }



        public override string ToString()
        {
            return this.artiest.Name + this.StageId;
        }

    }
}