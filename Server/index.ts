import express from 'express';
import { genkit }  from 'genkit';
import { ollama } from 'genkitx-ollama';
import bodyParser from 'body-parser';

const app = express();
const port = 8080;



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


app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});