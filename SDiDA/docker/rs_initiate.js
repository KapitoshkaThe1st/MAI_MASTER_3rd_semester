console.log("Replica set inititiation..."); 

var initiationNeeded = false;

try {
    rs.status();
} catch (e) {
    if(e.message == "no replset config has been received"){
        initiationNeeded = true;
    }
    else{
        console.log(`error = ${e}`);
    }
}

if(initiationNeeded) {
    let rsConf = {
        _id: "rs",
        members: [
            { _id: 0, host: "mongo0:27017" },
            { _id: 1, host: "mongo1:27017" },
            { _id: 2, host: "mongo2:27017" }
        ]
    }
    
    console.log(`rsConf = ${rsConf}`); 
    
    rs.initiate(rsConf)
}
else{
    console.log("No initiation needed")
}

console.log("Done"); 
