
server.log("----------------------------------");
server.log("----------[Bank Robery UI]--------");
server.log("-------[Successfully Loaded]------");
server.log("----------[Version " + Version + "]---------");
server.log("--------[Script By: Tanese]-------");
server.log("----------[UI By: Heawen]---------");
server.log("----------------------------------");

// Version
Version = "1.0.1";

/*
    Permissions:

    Permission for Robbers:
    Bank Info Permission: av.bankinfo
    Rob Permission: av.rob
    Rob Assist Permission: av.robassist

    Permission for Admins:
    Check Position Permission: av.checkpos

    Permissions for Debug (might become a feature):
    Command: bankdebug | Permission = av.debug
    Command: unbankdebug | Permission = av.debug

*/

// BANK INFO
bankName = "AV Bank"; // Change this to your bank's name (Will be displayed in announcements)
bankNameConfig = "av_bank"; // Put this in all lowercase letters and use "_" instead of spaces (You can shorten it to just "av") (Will be used in /rob <bankNameConfig> command)
bankRange = 50; // The range of your bank from the middle position

// BANK POSITION (Try to get to the middle of the bank)
x = 10; // Change this to your x position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the most accurate spot)
y = 10; // Change this to your y position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the most accurate spot)
z = 10; // Change this to your z position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the most accurate spot)


// CONFIGURATION
robTime = 120; // Put in Seconds
rewardRobExp = true; // Set to false if you don't want the robbers to be rewarded exp
rewardRobItems = true; // Set to false if you don't want the robbers to be rewarded items
robRewardExp = 10000; // Put Experience Reward
robRewardItems = [12, 13, 14]; // Put Item IDs for Reward

/*

    Don't change anything from here!!!

*/

effectId = 45818;
effectKey = 1294;

isBankOn = true;

config = {
    "Permission_Prefix": "av",
    "RobbingRange": bankRange
};

Robbing = array();

command bank(){
    permission = config["Permission_Prefix"] + ".bankinfo";
    allowedCaller = "player";
    execute(){
        player.message("The rob command is /rob " + bankNameConfig);
    }
}

command rob(location){
    permission = config["Permission_Prefix"] + ".rob";
    allowedCaller = "player";
    execute(){
        if(arguments.count < 1){
            player.message("Improper Usage! Use: /Rob " + bankNameConfig, "red");
            return;
        }
        if(Robbing != null){
            player.message("Someone is already robbing this bank");
            return;
        }
        else if(location == bankNameConfig){
            if(Robbing != null){
                player.message("Someone is already robbing this bank");
                return;
            }
            else if(isBankOn == false){
                player.message("You can't rob this bank since it is on cooldown!", "red");
                return;
            }
            else if(isBankOn == true){
                playerPos = player.position;
                bankPos = vector3(x, y, z);
                robbingRange = config["RobbingRange"];
                location = bankName;
                if(playerPos.distance(bankPos) <= robbingRange){
                    broadcast(player.name + " Is Robbing The " + location + "!", "orange");
                    wait.seconds(robTime, robover(player));
                }
                else{
                    player.message("You need to be atleast " + config["RobbingRange"] + " meters from " + location + "!", "red");
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
            player.message("Improper Usage! Use: /robassist <player>", "red");
            return;
        }
        argPlayer = toPlayer(argPlayer);
        if(argPlayer == null){
            player.message("Player not found!", "red");
        }
        else if(Robbing.contains(player.id)){
            player.message("You are already assisting in a robbery!", "red");
            return;
        }
        else if(Robbing.contains(argPlayer.id)){
            robberPos = argPlayer.position;
            playerPos = player.position;
            robbingRange = config["RobbingRange"];
            if(playerPos.distance(RobberPos) < robbingRange){
                broadcast(player.name + " Is now Assisting " + argPlayer.name + " in the robbery!", "red");
                Robbing.add(player.id);
                wait.seconds(600, robover(player));
            }
            else{
                player.message("You need to be atleast " + robbingRange + " meters from the Robber!", "red");
            }
        }
    }
}

function uihandle(){
    player.message("silly", "red");
}

function robover(player){
    if(Robbing.contains(player.id)){
        broadcast(player.name + " has finished their robbery on " + bankName + " succesfully!", "orange");
        Robbing.remove(player.id);
        givexp(player);
        giveitems(player);
    }
    else{
        player.message("You lost a robbery!", "red");
    }
}

function givexp(player){
    if(rewardRobExp == true){
        player.experience += robRewardExp;
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
            broadcast(player.name + " Has died while robbing the " + bankName + " and lost the robbery! He died by " + cause);
            Robbing.remove(player.id);
            wait.seconds(2, robover(player));
        }
        else if(killer != null){
            broadcast(player.name + " Has died while robbing the " + bankName + " and lost the robbery! The player that killed him was " + killer.name + "!");
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

// Debug
event onInterval(1){
    foreach(player in server.players){
        if(player.getData(bankkDebug) == true){
            playerPos = player.position;
            bankPos = vector3(x, y, z);
            robbingRange = config["RobbingRange"];
            if(playerPos.distance(bankPos) <= robbingRange){
                if(player.getData(isOnBank) == true){
                    player.message("You Entered the bank region!");
                    player.setData(isOnBank, false);
                }
            }
            else if(playerPos.distance(bankPos) >= robbingRange){
                if(player.getData(isOnBank) == false){
                    player.message("You Left the" + bankName +" region!");
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
//
