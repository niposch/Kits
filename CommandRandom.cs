using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fr34kyn01535.Kits
{
    public class CommandRandom : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return Rocket.API.AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "random"; }
        }

        public string Help
        {
            get { return "Use this to get a random kit"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "rdm" }; }
        }

        public bool RunFromConsole
        {
            get { return false; }
        }
        public List<string> Permissions
        {
            get { return new List<string> { "kit.random" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var random = new Random();

            UnturnedPlayer player = (UnturnedPlayer)caller;

            // check wether the player already has a kit
            if (Kits.Instance.PlayersWithKit.ContainsKey(caller.Id) && Kits.Instance.Configuration.Instance.OnlyOneKit)
            {
                UnturnedChat.Say(caller, Kits.Instance.Translations.Instance.Translate("command_kit_player_has_already_a_kit"));
                return ;
            }
            var availableKits = Kits.Instance.Configuration.Instance.Kits.Where(thisKit =>
            {

                bool hasPermissions = caller.HasPermission("kit.*") | caller.HasPermission("kit." + thisKit.Name.ToLower());
                if (!hasPermissions)
                {
                    return false;
                }
                KeyValuePair<string, DateTime> globalCooldown = Kits.GlobalCooldown.Where(k => k.Key == caller.ToString()).FirstOrDefault();
                if (!globalCooldown.Equals(default(KeyValuePair<string, DateTime>)))
                {
                    double globalCooldownSeconds = (DateTime.Now - globalCooldown.Value).TotalSeconds;
                    if (globalCooldownSeconds < Kits.Instance.Configuration.Instance.GlobalCooldown)
                    {
                        return false;
                    }
                }

                KeyValuePair<string, DateTime> individualCooldown = Kits.InvididualCooldown.Where(k => k.Key == (caller.ToString() + thisKit.Name)).FirstOrDefault();
                if (!individualCooldown.Equals(default(KeyValuePair<string, DateTime>)))
                {
                    double individualCooldownSeconds = (DateTime.Now - individualCooldown.Value).TotalSeconds;
                    if (individualCooldownSeconds < thisKit.Cooldown)
                    {
                        return false;
                    }
                }

                var cancelBecauseNotEnoughMoney = false;
                Kits.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) => {
                    Uconomy.Uconomy Uconomy = (Uconomy.Uconomy)plugin;
                    if ((Uconomy.Database.GetBalance(player.CSteamID.ToString()) + thisKit.Money.Value) < 0)
                    {
                        cancelBecauseNotEnoughMoney = true;
                        return;
                    }
                });
                if (cancelBecauseNotEnoughMoney)
                {
                    return false;
                }
                return true;
            });
            var availableCount = availableKits.Count();
            if(availableCount < 1)
            {
                UnturnedChat.Say(caller, "You do not have kits available to you right now. You may check /kits and see what's available.");
                return;
            }
            var kit = availableKits.ToList()[random.Next(availableCount)];
            var commandKit = new CommandKit();
            commandKit.Execute(caller, new string[] { kit.Name });
        }
    }
}
