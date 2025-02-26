import bodyParser from 'body-parser';
import express from 'express';
import {genkit} from 'genkit';
import {ollama} from 'genkitx-ollama';
import {MongoClient} from 'mongodb';

const app = express();
const port = 8080;

const client = new MongoClient("mongodb://root:root@mongo:27017")
await client.connect();
const database = client.db("test"); 
//initialize the database.
try {
  // Check if the collection already exists
  const collections = await database.listCollections({name: 'yourCollectionName'}).toArray();
  if (collections.length > 0) {
    console.log("Collection already exists.");
  }else{
    await database.createCollection("test");
  }



} catch(err){
  console.log(err);
}


app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

const ai = genkit({
    plugins: [
      ollama({
        models: [{ name: 'deepseek-r1' }],
        serverAddress: process.env.OLLAMAURL,
      }),
    ],
  });



//Route on test for testing if the server can respond
app.get("/test",(req, res) => {
    res.send("Read you loud and clear")
})

//Route to handle querys
app.post('/query', async (req, res) => {

    console.log(req.body)
    
    const { text } = await ai.generate({
        prompt: req.body.query,
        model: 'ollama/deepseek-r1',
      });

    console.log(text);
    res.send(text);
})



app.post("/InsertData",(req,res)=>{

  const collection = database.collection("users");
  collection.insertOne(req.body.doc);
  res.status(404).send("Inserted data"); 

})


app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});