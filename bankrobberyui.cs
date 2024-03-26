event onLoad(){
    server.log("----------------------------------");
    server.log("----------[Bank Robery UI]--------");
    server.log("-------[Successfully Loaded]------");
    server.log("----------[Version " + Version + "]---------");
    server.log("--------[Script By: Tanese]-------");
    server.log("----------[UI By: Heawen]---------");
    server.log("----------------------------------");
}


// Version
Version = "1.0.4";

/*
    Permissions:

    Permission for Robbers:
    Bank Info Permission: av.bankinfo
    Rob Permissi  on: av.rob
    Rob Assist Permission: av.robassist

    Permission for Admins:
    Check Position Permission: av.checkpos

    Permissions for Debug (might become a feature):
    Command: bankdebug | Permission = av.debug
    Command: unbankdebug | Permission = av.debug

*/

/*

   CONFIGURATION

*/

// BANK INFO
bankName = "AV Bank"; // Change this to your bank's name (Will be displayed in announcements)
bankNameConfig = "av_bank"; // Put this in all lowercase letters and use "_" instead of spaces (You can shorten it to just "av") (Will be used in /rob <bankNameConfig> command)
bankRange = 50; // The range of your bank from the middle position
bankCooldown = 300; // The cooldown after the bank got robbed
bankMessageIcon = "https://i.imgur.com/SFdYgXJ.png"; // Not necessary (Leave blank if you don't want)
// BANK POSITION (Try to get to the middle of your bank for the most accurate position)
x = 10; // Change this to your x position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the most accurate spot)
y = 10; // Change this to your y position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the most accurate spot)
z = 10; // Change this to your z position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the most accurate spot)
// CONFIGURATION
policeRocketId = PD; // Put your Police ROCKET Group Id here (Can be found in \Servers\YOURSERVER\Rocket\ and choose Permissions.config)
robberRange = 40; // The range a player has to be to join a robbery
robTime = 120; // Put in Seconds
rewardRobExp = true; // Set to false if you don't want the robbers to be rewarded exp
rewardRobItems = true; // Set to false if you don't want the robbers to be rewarded items
robRewardExp = 10000; // Put Experience Reward
robRewardItems = [12, 13, 14]; // Put Item IDs for Reward

/*

    TRANSLATIONS

*/

bankInfo_Translation = "The rob command is /rob {0}";
improperUsageRob_Translation = "Improper Usage! Use: /Rob {0}";
alreadyRobbing_Translation = "Someone is already robbing this bank!";
playerAlreadyRobbing_Translation = "You are already robbing this bank!";
robbingCooldown_Translation = "You can't rob this bank since it is on cooldown for {0} seconds!";
robbingOngoing_Translation = "{0} Is robbing the {1}!";
robbingTooFar_Translation = "You need to be atleast {0} meters from the {1} to rob it!";
improperUsageRobAssist_Translation = "Improper Usage! Use: /robassist <player>";
alreadyAssisting_Translation = "You are already assisting in a robbery!";
robberNotFound_Translation = "Robber not found!";
nowAssisting_Translation = "{0} Is now assisting {1} in the robbery!";
tooFarFromRobber_Translation = "You need to be atleast {0} meters from the robber to join him!";
robberySuccess_Translation = "{0} Has finished their robbery on {1} Succesfully!";
robberLost_Translation = "You lost a robbery!";
robberDiedNatural_Translation = "{0} Has died while robbing the {1} and lost the robbery! He died by {2}";
robberDiedPlayer_Translation = "{0} Has been killed while robbing the {1} and lost the robbery! The player that killed him was {2}";

/*

    Don't change anything from here if you don't know what you're doing!!!

*/

effectId = 45818;
effectKey = 1294;

isBankCooldownOn = false;
robCooldown = robTime;
robOngoing = false;

config = {
    "Permission_Prefix": "av",
    "RobbingRange": bankRange.toNumber();
};

Robbing = array();

command bank(){
    permission = config["Permission_Prefix"] + ".bankinfo";
    allowedCaller = "player";
    execute(){
        player.message(bankInfo_Translation.format(bankNameConfig), "green", bankMessageIcon);
    }
}

command rob(location){
    permission = config["Permission_Prefix"] + ".rob";
    allowedCaller = "player";
    execute(){
        if(arguments.count < 1){
            player.message(improperUsageRob_Translation.format(bankNameConfig), "red", bankMessageIcon);
            return;
        }
        if(location != bankNameConfig){
            player.message(improperUsageRob_Translation.format(bankNameConfig), "red", bankMessageIcon);
            return;
        }
        else if(location == bankNameConfig){
            if(Robbing.contains(player.id) or Robbing != null){
                if(Robbing.contains(player.id)){
                    player.message(playerAlreadyRobbing_Translation, "red", bankMessageIcon);
                    return;
                }
                else if(Robbing != null){
                    player.message(alreadyRobbing_Translation, "red", bankMessageIcon);
                    return;
                }
            }
            else if(isBankCooldownOn == true){
                player.message(robbingCooldown_Translation.format(bankCooldown), "red", bankMessageIcon);
                return;
            }
            else if(isBankCooldownOn == false){
                playerPos = player.position;
                bankPos = vector3(x, y, z);
                robbingRange = config["RobbingRange"];
                location = bankName;
                if(playerPos.distance(bankPos) <= robbingRange){
                    broadcast(robbingOngoing_Translation.format(player.name, location), "orange", bankMessageIcon);
                    robOngoing = true;
                    wait.seconds(robTime.toNumber, player.setData("robCompleted", true));
                }
                else{
                    player.message(robbingTooFar_Translation.format(config["RobbingRange"], location), "red", bankMessageIcon);
                    return;
                }
            }
        }
    }
}

command robassist(argPlayer){
    permission = config["Permission_Prefix"] + ".robassist";
    allowedCaller = "player";
    execute(){
        if(arguments.count < 1){
            player.message(improperUsageRobAssist_Translation, "red", bankMessageIcon);
            return;
        }
        argPlayer = toPlayer(argPlayer);
        if(argPlayer == null){
            player.message(robberNotFound_Translation, "red", bankMessageIcon);
            return;
        }
        else if(Robbing.contains(player.id)){
            player.message(alreadyAssisting_Translation, "red", bankMessageIcon);
            return;
        }
        else if(Robbing.contains(argPlayer.id)){
            robberPos = argPlayer.position;
            playerPos = player.position;
            robbingRange = config["RobbingRange"];
            if(playerPos.distance(RobberPos) < robberRange){
                broadcast(nowAssisting_Translation.format(player.name, argPlayer.name), "red", bankMessageIcon);
                Robbing.add(player.id);
                robOngoing = true;
                wait.seconds(robTime.toNumber, player.setData("robCompleted", true));
            }
            else{
                player.message(tooFarFromRobber_Translation.format(robberRange), "red", bankMessageIcon);
                return;
            }
        }
    }
}

function robberUI(player){
    player.message("silly", "red");
}

function copUI(player, robber){
    player.message("silly", "red");
}

function robover(player){
    if(Robbing.contains(player.id)){
        broadcast(robberySuccess_Translation.format(player.name, bankName), "orange", bankMessageIcon);
        Robbing.remove(player.id);
        givexp(player);
        giveitems(player);
        isBankCooldownOn = true;
    }
    else{
        player.message("You lost a robbery!", "red", bankMessageIcon);
        isBankCooldownOn = true;
    }
}

function givexp(player){
    if(rewardRobExp == true){
        player.experience += robRewardExp.toNumber();
    }
    else{
        return;
    }
}

function giveitems(player){
    if(rewardRobItems == true){
        foreach (item in robRewardItems){
            player.give(item, 1);
        }
    }
    else{
        return;
    }
}

event onPlayerDeath(player, killer, cause){
    if(Robbing.contains(player.id)){
        if(killer == null){
            broadcast(robberDiedNatural_Translation.format(player.name, bankName, cause), "orange", bankMessageIcon);
            Robbing.remove(player.id);
            wait.seconds(2, robover(player));
        }
        else if(killer != null){
            broadcast(robberDiedPlayer_Translation.format(player.name, bankName, killer.name), "orange", bankMessageIcon);
            Robbing.remove(player.id);
            wait.seconds(2, robover(player));
        }
    }
}

command checkposition(){
    permission = config["Permission_Prefix"] + ".checkpos";
    execute(){
        player.message("Position: vector3(x: {0}, y: {1}, z: {2}) | Check Console to get a copy-able version of this text".format(player.position.x,player.position.y,player.position.z));
        player.message("Rotation : {0} | Check Console to get a copy-able version of this text".format(player.rotation));
        logger.log("Position: vector3(x: {0}, y: {1}, z: {2}) ".format(player.position.x,player.position.y,player.position.z));
        logger.log("Rotation : {0}".format(player.rotation));
    }
}

event onInterval(1){
    foreach(player in server.players){
        if(robOngoing == true){
            if(player.getData("robCompleted") == true){
                robover(player);
            }
            else{
                return;
            }
        }
        else{
            return;
        }
    }
}

event onInterval(1){
    if(isBankCooldownOn == true){
        if(bankCooldown == 0){
            isBankCooldownOn = false;
        }
        else if(bankCooldown > 0){
            bankCooldown --;
        }
    }
    else{
        return;
    }
}

event onInterval(1){
    if(Robbing != null){
        if(robTime == 0){
            robTime = robCooldown;
        }
        else if(robTime > 0){
            robtime --;
        }
    }
    else{
        return;
    }
}

// Debug
event onInterval(1){
    foreach(player in server.players){
        if(player.getData(bankkDebug) == true){
            playerPos = player.position;
            bankPos = vector3(x, y, z);
            robbingRange = config["RobbingRange"];
            if(playerPos.distance(bankPos) <= robbingRange){
                if(player.getData(isOnBank) == true){
                    player.message("You Entered the" + bankName + "region!", "green", bankMessageIcon);
                    player.setData(isOnBank, false);
                }
            }
            else if(playerPos.distance(bankPos) >= robbingRange){
                if(player.getData(isOnBank) == false){
                    player.message("You Left the" + bankName +" region!", "green", bankMessageIcon);
                    player.setData(isOnBank, true);
                }
            }
        }
    }
}

command bankdebug(){
    permission = config["Permission_Prefix"] + ".debug";
    execute(){
        player.message("Warning! This feature is for debug purposes ONLY! It may cause your client to lag and your chat to be filled with messages!", "red");
        player.setData(bankkDebug, true);
    }
}

command unbankdebug(){
    permission = config["Permission_Prefix"] + ".debug";
    execute(){
        player.message("You have turned off debug mode!", "red");
        player.setData(bankkDebug, false);
    }
}
// Debug
