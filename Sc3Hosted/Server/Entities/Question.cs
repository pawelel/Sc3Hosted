namespace Sc3Hosted.Server.Entities;

public class Question
{
    public int QuestionId { get; set; }
    public string Name { get; set; }

    public Question(string name)
    {
        Name = name;
    }
}