// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

class Program
{
    class RegistrationResponse
    {
        public string? Login { get; set; }
        public string? ProfileId { get; set; }
    }

    class LoginResponse
    {
        public string? JwtToken { get; set; }
    }

    class PostCreationResponse
    {
        public string? PostId { get; set; }
        public string? AuthorId { get; set; }
        public string? Text { get; set; }
    }
    class CommentCreationResponse
    {
        public string? CommentId { get; set; }
        public string? PostId { get; set; }
        public string? AuthorId { get; set; }
        public string? Text { get; set; }
    }

    public static async Task Main()
    {
        //Console.WriteLine("Hello, World!");

        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "Your Oauth token");

        var jsonContent = JsonContent.Create(new { test = 1, a = "fergre" });
        var response = await httpClient.PostAsync("https://httpbin.org/post", jsonContent);

        Console.WriteLine(await response.Content.ReadAsStringAsync());

        ////Dictionary<int, string> idToId = new Dictionary<int, string>();
        ////Dictionary<int, string> idToId = new Dictionary<int, string>();
        ////Dictionary<int, string> idToId = new Dictionary<int, string>();

        //var data = new { Name = "Tom", Age = 38 };
        //// создаем JsonContent
        //JsonContent content = JsonContent.Create(data);

        //var response = await httpClient.PostAsync("https://httpbin.org/post", content);

        //Console.WriteLine($"headers={response.Headers}\ncontent={await response.Content.ReadAsStringAsync()}");

        string userStr =
        @"<row 
    Id=""-1"" 
    Reputation=""1"" 
    CreationDate=""2009-07-12T22:51:42.563"" 
    DisplayName=""Community""
    LastAccessDate=""2008-07-31T00:00:00.000"" 
    WebsiteUrl=""http://meta.stackexchange.com/""
    Location=""on the server farm""
    AboutMe=""&lt;p&gt;Hi, I'm not really a person.&lt;/p&gt;&#xA;&#xA;&lt;p&gt;I'm a background process that helps keep this site clean!&lt;/p&gt;&#xA;&#xA;&lt;p&gt;I do things like&lt;/p&gt;&#xA;&#xA;&lt;ul&gt;&#xA;&lt;li&gt;Randomly poke old unanswered questions every hour so they get some attention&lt;/li&gt;&#xA;&lt;li&gt;Own community questions and answers so nobody gets unnecessary reputation from them&lt;/li&gt;&#xA;&lt;li&gt;Own downvotes on spam/evil posts that get permanently deleted&lt;/li&gt;&#xA;&lt;li&gt;Own suggested edits from anonymous users&lt;/li&gt;&#xA;&lt;li&gt;&lt;a href=&quot;http://meta.stackexchange.com/a/92006&quot;&gt;Remove abandoned questions&lt;/a&gt;&lt;/li&gt;&#xA;&lt;/ul&gt;&#xA;""
    Views=""19921"" 
    UpVotes=""53743"" 
    DownVotes=""193678"" 
    AccountId=""-1"" 
/>";

        string postStr =
        @"<row
    Id=""65""
    PostTypeId=""1""
    CreationDate=""2009-07-15T07:25:28.783""
    Score=""24""
    ViewCount=""58346""
    Body=""&lt;p&gt;I got a keyboard (&lt;a href=&quot;http://www.logitech.com/index.cfm/keyboards/keyboard/devices/3071&amp;amp;cl=US,EN&quot;&gt;Logitech Wave&lt;/a&gt;, pictured below) which I'm very happy with. Unfortunately, the manufacturer has changed the button for right-click for a FN key.&lt;/p&gt;&#xA;&#xA;&lt;p&gt;Is there any program to remap the FN key to something else?&lt;/p&gt;&#xA;&#xA;&lt;p&gt;&lt;img src=&quot;https://i.stack.imgur.com/Ls7uQ.jpg&quot; alt=&quot;alt text&quot;&gt;&lt;/p&gt;&#xA;""
    OwnerUserId=""49""
    LastEditorUserId=""73637""
    LastEditDate=""2011-08-20T05:44:03.923""
    LastActivityDate=""2020-07-07T06:19:40.043""
    Title=""Remap FN to another key""
    Tags=""&lt;keyboard&gt;&lt;keymap&gt;&lt;function-keys&gt;""
    AnswerCount=""2""
    CommentCount=""12""
    ContentLicense=""CC BY-SA 3.0""
/>";

        string commentStr =
        @"<row
    Id=""1522"" 
    PostId=""2574"" 
    Score=""2""
    Text=""Also keep in mind &gt;&gt; and &lt;&lt; which can manually do (un)indenting.""
    CreationDate=""2009-07-15T17:46:50.717"" 
    UserId=""1314"" 
    ContentLicense=""CC BY-SA 2.5"" 
/>";


        string? GetItem(XmlElement? xRoot, string fieldName)
        {
            return xRoot?.Attributes.GetNamedItem(fieldName)?.Value;
        }

        async Task ProcessLine(string line, Func<XmlElement, Task> action)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(line);

            XmlElement? xElement = xDoc.DocumentElement;
            if (xElement == null)
            {
                return;
            }
            await action(xElement);
        }

        async Task ProcessData(string filePath, Func<XmlElement, Task> action, int nRows = -1)
        {
            using (var sr = new StreamReader(filePath))
            {
                sr.ReadLine();
                sr.ReadLine();

                int rowsProcessed = 0;

                string? line;
                while (true)
                {
                    Console.WriteLine($"rows processed: {rowsProcessed}");

                    if (nRows > 0 && nRows == rowsProcessed)
                    {
                        break;
                    }

                    if ((line = sr.ReadLine()) == null)
                    {
                        break;
                    }

                    bool eof = sr.Peek() == -1;
                    if (eof)
                    {
                        break;
                    }

                    await ProcessLine(line, action);

                    rowsProcessed++;
                }
            }
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        string PreprocessText(string text)
        {
            return StripTagsRegexCompiled(
                    text
                    .Replace("&lt", "<")
                    .Replace("&gt", ">")
                )
                .Replace("&#xA", "")
                .Replace(";", "")
                .Replace("\n", "\\n");
        }

        //string registrationUrl = "https://httpbin.org/post";

        string dadaDirectoryPath = "..//..//..//data//";
        string usersFilePath = dadaDirectoryPath + "Users.xml";
        string postsFilePath = dadaDirectoryPath + "Posts.xml";
        string commentsFilePath = dadaDirectoryPath + "Comments.xml";

        string baseUrl = "https://localhost:5001/api/";
        string registrationUrl = baseUrl + "registration/";
        string loginUrl = baseUrl + "login/";
        string postsUrl = baseUrl + "posts";
        string commentsUrl = baseUrl + "comments";

        Random rnd = new Random();

        string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!_.?<>,@#$%^&-+=:;`~()";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        string GeneratePassword()
        {
            return RandomString(10);
        }

        var userIdMap = new Dictionary<int, string>();
        var postIdMap = new Dictionary<int, string>();
        var jwtTockensByIdMap = new Dictionary<int, string>();

        Console.WriteLine("Press enter to continue");
        Console.ReadLine();

        Console.WriteLine("POPULATE USERS");

        // Populate users
        await ProcessData(usersFilePath, async e =>
        {
            string? id = GetItem(e, "Id");
            string? displayName = GetItem(e, "DisplayName");
            string? aboutMe = GetItem(e, "AboutMe");

            if (id == null || displayName == null || aboutMe == null)
            {
                return;
            }

            int localId = Convert.ToInt32(id);

            var login = displayName;
            var password = GeneratePassword();

            var content = new
            {
                Login = login,
                Password = password,
                Username = login,
                Status = PreprocessText(aboutMe),
            };

            var jsonContent = JsonContent.Create(content);

            string s = await jsonContent.ReadAsStringAsync();

            Console.WriteLine(s);

            var response = await httpClient.PostAsync(registrationUrl, jsonContent);

            Console.WriteLine("RESPONSE:");
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            var registrationResponse = await response.Content.ReadFromJsonAsync<RegistrationResponse>();

            if (registrationResponse == null)
            {
                throw new Exception("WTF???");
            }

            userIdMap[localId] = registrationResponse.ProfileId;

            jsonContent = JsonContent.Create(
                new
                {
                    Login = login,
                    Password = password
                });

            response = await httpClient.PostAsync(loginUrl, jsonContent);

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            jwtTockensByIdMap[localId] = loginResponse.JwtToken;
        });

        Console.WriteLine("POPULATE POSTS");

        // Populate posts
        await ProcessData(postsFilePath, async e =>
        {
            string? id = GetItem(e, "Id");
            string? ownerId = GetItem(e, "OwnerUserId");
            string? body = GetItem(e, "Body");

            if (ownerId == null || body == null)
            {
                return;
            }

            int localId = Convert.ToInt32(id);
            int localOwnerId = Convert.ToInt32(ownerId);

            var content = new
            {
                AuthorId = userIdMap[localOwnerId],
                Text = PreprocessText(body)
            };

            var jsonContent = JsonContent.Create(content);

            string s = await jsonContent.ReadAsStringAsync();

            // Console.WriteLine(s);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtTockensByIdMap[localOwnerId]);

            var response = await httpClient.PostAsync(registrationUrl, jsonContent);

            // Console.WriteLine("RESPONSE:");
            // Console.WriteLine(await response.Content.ReadAsStringAsync());

            var jsonResponse = await response.Content.ReadFromJsonAsync<PostCreationResponse>();

            if (jsonResponse == null)
            {
                throw new Exception("WTF???");
            }

            postIdMap[localId] = jsonResponse.PostId;
        });

        Console.WriteLine("POPULATE COMMENTS");

        // Populate comments
        await ProcessData(commentsFilePath, async e =>
        {
            string? userId = GetItem(e, "UserId");
            string? text = GetItem(e, "Text");
            string? postId = GetItem(e, "PostId");

            if (userId == null || text == null || postId == null)
            {
                return;
            }

            int localUserId = Convert.ToInt32(userId);
            int localPostId = Convert.ToInt32(postId);

            var content = new
            {
                AuthorId = userIdMap[localUserId],
                PostId = userIdMap[localPostId],
                Text = PreprocessText(text)
            };

            var jsonContent = JsonContent.Create(content);

            string s = await jsonContent.ReadAsStringAsync();

            // Console.WriteLine(s);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtTockensByIdMap[localUserId]);

            var response = await httpClient.PostAsync(registrationUrl, jsonContent);

            // Console.WriteLine("RESPONSE:");
            // Console.WriteLine(await response.Content.ReadAsStringAsync());

            var jsonResponse = await response.Content.ReadFromJsonAsync<PostCreationResponse>();

            if (jsonResponse == null)
            {
                throw new Exception("WTF???");
            }
        });

        Console.ReadLine();
    }
}