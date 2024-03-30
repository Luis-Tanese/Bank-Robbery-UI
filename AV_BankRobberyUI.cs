event onLoad(){
    server.log("----------------------------------");
    server.log("--------[AV Bank Robery UI]--------");
    server.log("-------[Successfully Loaded]------");
    server.log("----------[Version " + Version + "]---------");
    server.log("--------[Script By: Tanese]-------");
    server.log("----------------------------------");
}


// Version
Version = "1.1.0";

/*
    Permissions:

    Permission for Robbers:
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

bankInfo = [ // All your bank's info (If you don't know what to do or get errors, ask me 'tanese' on discord for help!)
    [
        "AV Bank", // Change this to your bank's name (Will be displayed in UI)
        vector3(80.7193984985352, 34.5396766662598, 810.215026855469),  // Center point of your bank region. (Use /checkposition to get position).
        100, // The range of your bank from the center position
        300, // The cooldown after your bank got robbed
        300 // The time robbers take to finish robbing the bank
    ],
    [
        "Silly Bank", // Change this to your bank's name (Will be displayed in UI)
        vector3(45.7193, 90.5396598, 1100.268469), // Center point of your bank region. (Use /checkposition to get position).
        100, // The range of your bank from the center position
        300, // The cooldown after your bank got robbed
        300 // The time robbers take to finish robbing the bank
    ],
    [
        "Womp Bank", // Change this to your bank's name (Will be displayed in UI)
        vector3(12.939842, 67.6662, 810.2159), // Center point of your bank region. (Use /checkposition to get position).
        100, // The range of your bank from the center position
        300, // The cooldown after your bank got robbed
        300 // The time robbers take to finish robbing the bank
    ]
];
// Important Details
policeRocketId = PD; // Put your Police ROCKET Group Id here (Can be found in \Servers\YOURSERVER\Rocket\ and choose Permissions.config)
robberRange = 20; // The range a player has to be from another player to join a robbery
rewardRobItems = true; // Set to false if you don't want the robbers to be rewarded items
robRewardItems = [12, 13, 14]; // Put Item IDs for Reward
bankMessageIcon = "https://i.imgur.com/SFdYgXJ.png"; // Not necessary (Leave blank if you don't want)

/*

    TRANSLATIONS

*/

playerTooFarBank_Translation = "You are too far from the bank to use this command!";
bankOnCooldown_Translation = "The bank is still in cooldown for {0} seconds!";
alreadyRobbingBank_Translation = "You are already robbing this bank!";
nowRobbingBank_Translation = "{0} is now robbing the {1}!";
finishedRobbingBank_Translation = "{0} has successfully robbed the {1}!";
robberLostBank_Translation = "You lost this robbery!";
incorrectUsageAssist_Translation = "Incorrect Usage! Use /robassist <player>";
noPlayerFound_Translation = "No player found!";
alreadyAssisting_Translation = "You are already assisting in a robbery!";
nowAssisting_Translation = "{0} is now assisting {1} in the robbery of {2}!";
someoneAlreadyRobbing_Translation = "Someone is already robbing this bank!";
robberDiedNatural_Translation = "{0} Has died while robbing the {1} and lost the robbery! He died by {2}";
robberDiedPlayer_Translation = "{0} Has been killed while robbing the {1} and lost the robbery! The player that killed him was {2}";
diedWhileAssisting_Translation = "You died while Assisting someone, you will have to assist them again in order to go back to the robbery!";

/*

    Don't change anything from here if you don't know what you're doing!!!

*/

efffectId = 45818;
Robbing = [];

command rob(){
    permission = "av.rob";
    allowedCaller = "player";
    execute(){
        bankID = getNearestBank(player);
        if(player.position.distance(bankInfo[bankID][1]) > bankInfo[bankID][1]){
            player.message(playerTooFarBank_Translation, "red", bankMessageIcon);
            return;
        }
        else if(player.position.distance(bankInfo[bankID][1]) < bankInfo[bankID][2]){
            bankCooldown = bankInfo[bankID][3];
            robTime = bankInfo[bankID][4];
            if(bankCooldown == bankInfo[bankID][3]){
                bankName = bankInfo[bankID][0];
                if(Robing.contains(player.id)){
                    player.message(alreadyRobbingBank_Translation, "red", bankMessageIcon);
                }
                else if(Robbing.contains(bankName)){
                    player.message(someoneAlreadyRobbing_Translation, "red", bankMessageIcon);
                    return;
                }
                else if(!Robbing.contains(player.id)){
                    Robbing.add(bankName, player.id);
                    broadcast(nowRobbingBank_Translation.format(player.name, bankInfo[bankID][0]), "orange", bankMessageIcon);
                    countdown(robTime, robOver(player, bankCooldown, bankName));
                }
            }
            else{
                player.message(bankOnCooldown_Translation.format(bankCooldown), "red", bankMessageIcon);
            }
        }
        else{
            return;
        }
    }
}

command robassist(robber){
    permission = "av.robassist";
    allowedCaller = "player";
    execute(){
        bankID = getNearestBank(player);
        if(arguments.count < 1){
            player.message(incorrectUsageAssist_Translation, "red", bankMessageIcon);
            return;
        }
        robber = toPlayer(robber);
        if(argPlayer == null){
            player.message(noPlayerFound_Translation, "red", bankMessageIcon);
            return;
        }
        if(player.position.distance(bankInfo[bankID][1]) > bankInfo[bankID][1]){
            player.message(playerTooFarBank_Translation, "red", bankMessageIcon);
            return;
        }
        else if(player.position.distance(bankInfo[bankID][1]) < bankInfo[bankID][2]){
            bankName = bankInfo[bankID][0];
            if(player.getData("Assisting") == true){
                player.message(alreadyAssisting_Translation, "red", bankMessageIcon);
                return;
            }
            if(player.position.distance(robber.position) < robberRange){
                Robbing.add(player.id);
                broadcast(nowAssisting_Translation.format(player.name, robber.name, bankName), "orange", bankMessageIcon);
                player.setData("Assisting", true);
            }
        }
    }
}

event onPlayerDeath(player, killer, cause){
    bankID = getNearestBank(player);
    if(Robbing.contains(player.id)){
        if(killer == null){
            broadcast(robberDiedNatural_Translation.format(player.name, bankInfo[bankID][0], cause), "orange", bankMessageIcon);
            Robbing.remove(player.id);
            wait.seconds(2, robover(player));
        }
        else if(killer != null){
            broadcast(robberDiedPlayer_Translation.format(player.name, bankInfo[bankID][0], killer.name), "orange", bankMessageIcon);
            Robbing.remove(player.id);
            wait.seconds(2, robover(player));
        }
    }
    else if(player.getData("Assisting") == true){
        player.message(diedWhileAssisting_Translation, "red", bankMessageIcon);
        player.setData("Assisting", false);
        return;
    }
}

function getNearestBank(player){
    for(
        bank = 0;
        bank < bankInfo.count;
        bank ++
    ){
        if(player.position.distance(bankInfo[bank][1]) <= bankInfo[bank][2]){
            return bank;
        }
    }
    return null;
}

function resetBankCooldown(player, bankCooldown){
    bankID = getNearestBank(player);
    bankCooldown = bankInfo[bankID][3];
}

function robOver(player, bankCooldown, bankName){
    bankID = getNearestBank(player);
    foreach(player in server.players){
        if(Robbing.contains(bankName, player.id)){
            broadcast(finishedRobbingBank_Translation.format(player.name, bankInfo[bankID][0]), "orange", bankMessageIcon);
            Robbing.remove(bankName, player.id);
            givexp(player);
            giveitems(player);
            resetBankCooldown(player, bankCooldown);
            return;
        }
        else if(player.getData("Assisting") == true and !Robbing.contains(player.id)){
            givexp(player);
            giveitems(player);
            player.setData("Assisting", false);
            return;
        }
        else{
            return;
        }
    }

    if(Robbing.contains(bankName, player.id)){
        broadcast(finishedRobbingBank_Translation.format(player.name, bankInfo[bankID][0]), "orange", bankMessageIcon);
        Robbing.remove(player.id);
        
        giveitems(player);
        resetBankCooldown(player, bankCooldown);
        return;
    }
    else if(player.getData("Assisting") == true and !Robbing.contains(player.id)){
        givexp(player);
        giveitems(player);
        Robbing.remove(player.id);
        return;
    }
    else{
        player.message(robberLostBank_Translation);
        return;
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

function countdown(time, action){
  countdownHelper(time, 0, action);
}

function countdownHelper(time, current, action){
    if(time == current){
        action();
        return;
    }
    else{
        diff = time - current;
        mins = math.floor(diff / 60);
        seconds = diff % 60;
        if(mins <= 9){
            mins = "0" + mins;
        }
        if(seconds <= 9){
            seconds = "0" + seconds;
        }
        print("{0}:{1}".format(mins, seconds));
        wait.seconds(1, countdownHelper, time, current + 1, action);
    }
}


// Debug 
event onInterval(1){
    foreach(player in server.players){
        if(player.getData("BankDebug") == true){
            playerPos = player.position;
            bankPos = bankInfo[bankID][1];
            bankRange = bankInfo[bankID][2];
            bankName = bankInfo[bankID][0];
            if(player.position.distance(bankPos) < bankRange){
                if(player.getData(isOnBank) == true){
                    player.message("You Entered the" + bankName + "region!", "green", bankMessageIcon);
                    player.setData(isOnBank, false);
                    return;
                }
                else{
                    return;
                }
            }
            else if(playerPos.distance(bankPos) >= bankRange){
                if(player.getData(isOnBank) == false){
                    player.message("You Left the" + bankName +" region!", "green", bankMessageIcon);
                    player.setData(isOnBank, true);
                    return;
                }
                else{
                    return;
                }
            }
        }
        else{
            return;
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