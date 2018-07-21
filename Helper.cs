using System;
using Microsoft.AspNetCore.Mvc;

// this is broken right now
// https://www.codeproject.com/Tips/780818/Using-TempData-Outside-the-Controller-in-ASP-NET-M
public class Helper {
    public static void SetPrompt(Controller controller, bool success,string message)
    {
        try {
            PromptNotification notifier = new PromptNotification
            {
                NotificationSuccess = success,
                NotificationMessage = message
            };
            controller.TempData["PromptNotification"] = notifier;
        }
        catch {
            Console.WriteLine("SET PROMPT catch");
        }
    }

    public static PromptNotification GetPrompt(Controller controller)
    {
        try
        {
            PromptNotification notifier = controller.TempData["PromptNotification"]
                as PromptNotification;
            return notifier;
        }
        catch (Exception ex)
        {
            Console.WriteLine("this is where the error would go");
            Console.WriteLine(ex);

            return null;
        }
    }

    public static void RemovePrompt(Controller controller)
    {
        controller.TempData.Remove("PromptNotification");
    }
}
