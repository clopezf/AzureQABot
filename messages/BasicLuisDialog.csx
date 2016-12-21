using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using QC = System.Data.SqlClient;  // System.Data.dll  
using DT = System.Data;  // System.Data.dll 

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the «none» intent. You said: {result.Query}"); //
        context.Wait(MessageReceived);
    }

    // Go to https://luis.ai and create a new intent, then train/publish your luis app.
    // Finally replace "MyIntent" with the name of your newly created intent in the following handler
    [LuisIntent("Greetings")]
    public async Task GreetingsIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Welcome to Azure Athlon Challenge"); //
        context.Wait(MessageReceived);
    }


    [LuisIntent("Confirmation")]
    public async Task ConfirmationIntent(IDialogContext context, LuisResult result)
    {
        string questionIdToUser;
        string questionTypeToUser;
        string questionToUser;
        string questionToAnswer;
        string questionToAnswer1;
        string questionToAnswer2;
        string questionToAnswer3;

        using (var connection = new QC.SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["connectiondb"]))
        {
            connection.Open();

            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"  
      SELECT TOP (1) [id_question]
      ,[type]
      ,[questions]
	  ,[answer]
	  ,[answer_1]
	  ,[answer_2]
	  ,[answer_3]
	  ,[subtype]
  FROM [dbo].[santabot_questions]
  ORDER BY NEWID();";


                QC.SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                questionIdToUser = reader.GetString(0);
                questionTypeToUser = reader.GetString(1);
                questionToUser = reader.GetString(2);
                questionToAnswer = reader.GetString(3);
                questionToAnswer1 = reader.GetString(4);
                questionToAnswer2 = reader.GetString(5);
                questionToAnswer3 = reader.GetString(6);
            }

        }

        await context.PostAsync($"Good, you must know a good Elf always read about Christmas and know important facts. Ready, set, Go!!");
        await context.PostAsync(questionToUser);
        await context.PostAsync($"A. " + questionToAnswer);
        await context.PostAsync($"B. " + questionToAnswer1);
        await context.PostAsync($"C. " + questionToAnswer2);
        await context.PostAsync($"D. " + questionToAnswer3);

        context.ConversationData.SetValue("lastquestion", questionIdToUser);

        string lastQ = string.Empty;
        context.ConversationData.TryGetValue<string>("lastquestion", out lastQ);

        await context.PostAsync($"Last question: " + lastQ);
        context.Wait(MessageReceived);
    }
}
