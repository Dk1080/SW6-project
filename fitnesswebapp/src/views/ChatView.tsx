import { useEffect, useState } from "react";
import ChatMessenge from "../components/ChatMessenge";

function ChatView() {

    const [chatlog, setChatlog] = useState();

    //Flag for not sending the same request twice
    let requestFlag = false;


    //Ask for full chatlog from server on page load.
    useEffect(() => {
        const getChartData = async () => {
            if (!requestFlag) {
                requestFlag = true;
                await fetch("/api/getChats")
                    .then(response => response.json())
                    .then(data => {
                        console.log(data);
                        setChatlog(data);
                        
                    }) }
        };

        getChartData();
    }, []);



    //populate the current box with top chat.



    //When user sends a query send it to server append that and the response.




    //Show loading messesasge while waiting for chatlog
    if (chatlog == null) {
        return (
            <div>
                <h1>Loading...</h1>
            </div>
        );
    } else {
        
        return (
            <div>
                {chatlog?.histories?.[0]?.chatHistory?.map((item) => (
                    <ChatMessenge item={item} />
                ))}
            </div>
        )
    }
}

export default ChatView;