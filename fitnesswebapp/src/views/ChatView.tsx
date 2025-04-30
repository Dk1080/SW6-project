import { useEffect, useState } from "react";
import ChatMessenge from "../components/ChatMessenge";

function ChatView() {

    const [chatlog, setChatlog] = useState();
    const [currentChat, setCurrentChat] = useState();
    const [query, setQuery] = useState("");
    const [threadId, setThreadId] = useState();

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
                        setChatlog(data);

                    })
            }
        };

        getChartData();
    }, []);

    //Rerender the page to show the first chat.
    useEffect(() => {
        if (chatlog?.histories?.[0]?.chatHistory && !threadId) {
            // Set the starting chat to be the first one in the array.
            setThreadId(chatlog.histories[0].id);
            setCurrentChat(chatlog.histories[0].chatHistory);
        }
    }, [chatlog, threadId]);

  

    const changeChat = (dessiredChat: object) => {

        setThreadId(dessiredChat.id);

        setCurrentChat(dessiredChat.chatHistory);
    }

    const sendQuery = async (query) => {

        //Create temperary chat.
        const updatedChatlog = { ...chatlog };


       // console.log(chatlog)
        console.log(query)
        //loop through each chat until we find the current chat
        for (const chat of updatedChatlog.histories) {
            if (chat.id === threadId) {
                console.log("here " + threadId)
                //Add the query to the current chat
                chat.chatHistory.push({
                    "authorName": null,
                    "role": "user",
                    "text": query
                });
                console.log(chat);


                setChatlog(updatedChatlog);

                //Send the query
                const body = {
                    "query": query,
                    "threadId": threadId,
                    "role": "user"
                }
                console.log(body)

                await fetch("/api/chat", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(body)
                })
                .then(response => {
                    if (response.ok) {
                        //Turn the promise into a usable string.
                        response.text().then((resolvedString: string) => {
                            const regularString: string = resolvedString;

                            console.log(JSON.parse(regularString));
                            const AIresponse = JSON.parse(regularString);
                            //Add the response to the current chat.
                            chat.chatHistory.push({
                                "authorName": null,
                                "role": "assistant",
                                "text": AIresponse.query
                            });

                            //Update and force a rerender.
                            setChatlog({ ...updatedChatlog });});

                    }
                });
            }
        }

    }


    //function to open the side menu.
    const openMenu = () => {
        document.getElementById("sideMenu").classList.add("open");

    }

    const closeMenu = () => {
        document.getElementById("sideMenu").classList.remove("open");
    }




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


                <div className="sideMenu" id="sideMenu">
                    <button onClick={closeMenu}>close</button>

                    {chatlog?.histories?.map((item) => {
                        return <button onClick={() => changeChat(item)}>{item.id}</button>;
                    })}
                </div>

                <button onClick={openMenu}>Select chat logs</button>

                <div>
                    {currentChat?.map((item: any) => {
                        return <ChatMessenge item={item} />;
                    })}
                    <input type="text" onChange={e => setQuery(e.target.value)}></input>
                    <button onClick={() => sendQuery(query)}>Send query!</button>
                </div>



                
                


            </div>
        )
    }
}

export default ChatView;