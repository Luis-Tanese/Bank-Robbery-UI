
server.log("----------------------------------");
server.log("----------[KuyumcuSoygunu]--------");
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

*/

// BANK INFO
bankName = "AV Bank"; // Change this to your bank's name
bankNameConfig = "av_bank"; // Put this in all lowercase letters and use "_" instead of spaces
// BANK POSITION
x = 10; // Change this to your x position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the best spot)
y = 10; // Change this to your y position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the best spot)
z = 10; // Change this to your z position on Vector3 | Use Command /checkposition to Get Your Vector3 Coordinates (Use all characters for the best spot)


// CONFIGURATION
robTime = 600; // Put in Seconds
rewardRobExp = true; // Set to false if you don't want the robbers to be rewarded exp
rewardRobItems = true; // Set to false if you don't want the robbers to be rewarded items
robRewardExp = 100000; // Put Experience Reward
robRewardItems[12, 13, 14]; // Put Item IDs for Reward

/*

    Don't change anything from here!!!

*/

effectId = 45818; // DON'T CHANGE

config = {
    "Permission_Prefix": "av",
    "RobbingRange": 50
};
Robbing = array();

command bank(){
    permission = config["Permission_Prefix"] + ".bankinfo";
}

command rob(location){
    permission = config["Permission_Prefix"] + ".rob";
    allowedCaller = "player";
    execute(){
        if(arguments.count < 1){
            player.message("Improper Usage! Use: /Rob <Location>", "red");
            return;
        }
        if(Robbing != null){
            player.message("Someone is already robbing this bank");
            return;
        }
        if(location == bankNameConfig){
            playerPos = player.position;
            bankPos = vector3(x, y, z);
            robbingRange = config["RobbingRange"];
            location = bankName;
            if(playerPos.distance(bankPos) < robbingRange){
                broadcast(player.name + " Is Robbing The " + location + "!", "orange");
                Robbing.add(player.id);
                wait.seconds(600, robover(player));
            }
            else{
                player.message("You need to be atleast " + config["RobbingRange"] + " meters from " + location + "!", "red");
                return;
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
    a
}

function robover(player){
    if(Robbing.contains(player.id)){
        broadcast(player.name + " has finished their robbery succesfully!", "orange");
        Robbing.remove(player.id);
        if(rewardRobExp == true){
            player.experience += robRewardExp;
        }
        if(rewardRobItems == true){
            foreach(item in robRewardItems){
                player.give(item, 1);
            }
        }
    }
    else{
        player.message("You lost a robbery!", "red");
    }
}

event onPlayerDeath(victim, killer, cause){
    if(Robbing.contains(victim.id)){
        if(killer == null){
            broadcast(victim.name + " Has died while robbing the " + bankName + " and lost the robbery! He died by " + cause);
        }
        else if(killer != null){
            broadcast(victim.name + " Has died while robbing the " + bankName + " and lost the robbery! The player that killed him was " + killer.name + "!");
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
