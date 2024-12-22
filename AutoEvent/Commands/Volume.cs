using System;
using CommandSystem;
using Exiled.API.Features;
using SCPSLAudioApi.AudioCore;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands;
public class Volume : ICommand, IUsageProvider
{
    public string Command => nameof(Volume);
    public string Description => "Set the global music volume, takes on 1 argument - the volume from 0%-200%";
    public string[] Aliases => [];
    public string[] Usage => ["Volume %"];
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        try
        {
            if (!sender.CheckPermission("ev.volume"))
            {
                response = "<color=red>You do not have permission to use this command!</color>";
                return false;
            }
            if (arguments.Count != 1)
            {
                response = $"The current volume is {AutoEvent.Singleton.Config.Volume}%. Please specify the new volume from 0% - 200% to set it!";
                return false;
            }

            float newVolume = float.Parse(arguments.At(0));
            AutoEvent.Singleton.Config.Volume = newVolume;
            if (Extensions.AudioBot is not null)
            {
                var audioPlayer = AudioPlayerBase.Get(Extensions.AudioBot).Volume = newVolume;
            }

            response = $"The volume has been set!";
            return true;
        }
        catch (Exception e)
        {
            response = $"Could not set the volume due to an error. This could be a bug. Ensure audio is playing while using this command.";
            Log.Warn($"An error has occured while trying to set the volume.");
            Log.Debug($"{e}");
            return false;
        }
    }
}