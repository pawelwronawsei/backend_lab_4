using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto;

public class NewQuizDto
{
    [Microsoft.Build.Framework.Required]
    [Length(minimumLength: 3, maximumLength: 200)]
    public string Title { get; set; }
}