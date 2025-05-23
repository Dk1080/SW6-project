import { useEffect, useRef, useState } from "react";
import ChatMessenge from "../components/ChatMessenge";
import SpeechRecognition, { useSpeechRecognition } from "react-speech-recognition";
import { useNavigate } from "react-router";
function ChatView() {

    const navigate = useNavigate();
    const [chatlog, setChatlog] = useState();
    const [currentChat, setCurrentChat] = useState();
    const [query, setQuery] = useState("");
    const [threadId, setThreadId] = useState(-1);
    const firstChatFlag = useRef(false);

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
        if (chatlog?.histories?.length && firstChatFlag.current == false) {
            const first = chatlog.histories[0];
            setThreadId(first.id);
            setCurrentChat(first.chatHistory);
            firstChatFlag.current = true
            console.log(first.chatHistory);
        }
    }, [chatlog]);



    //Configure voice to text service.
    const {
        transcript,
        listening,
        resetTranscript,
    } = useSpeechRecognition();



    const changeChat = (dessiredChat: object) => {

        setThreadId(dessiredChat.id);

        setCurrentChat(dessiredChat.chatHistory);
    }

    const sendQuery = async (query) => {


        //Check if there is text in the input field.
        if (transcript.length == 0 && query.length == 0) {
            alert("Input query");
            return;
        }


        //Dissable the input and clear the box
        document.getElementById("queryButton").disabled = true;
        setQuery("");
        resetTranscript();


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

                            //If this is a new chat then add it to the chatlog
                            if (threadId == 0) {
                                //Set the new thread id
                                threadId = AIresponse.threadId;
                            }


                            //Update and force a rerender.
                            setChatlog({ ...updatedChatlog });});
                        return;
                    }
                });
            }
        }

        //If new chat
        if (threadId == 0) {
            //create a new chat
            console.log("here " + threadId)
            const newChat: object = {}
            newChat.chatHistory = []


            //Add the query to the new chat
            newChat.chatHistory.push({
                "authorName": null,
                "role": "user",
                "text": query
            });
            console.log(newChat);

            //Set the current chat to hold the newchat.
            setCurrentChat(newChat.chatHistory);

            //Send the query
            const body = {
                "query": query,
                "threadId": "000000000000000000000000",
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
                            newChat.chatHistory.push({
                                "authorName": null,
                                "role": "assistant",
                                "text": AIresponse.query
                            });
                            setCurrentChat(newChat.chatHistory);

                            //Set the new thread id
                            setThreadId(AIresponse.threadId);
                            newChat.id = AIresponse.threadId;

                            //Add the new chat to the chatlog
                            console.log(updatedChatlog);

                            updatedChatlog.histories.push(newChat)
                            console.log(updatedChatlog);

                            //Update and force a rerender.
                            setChatlog({ ...updatedChatlog });

                        });
                    }
                });
        }
        document.getElementById("queryButton").disabled = false;

        

    }


    //function to open the side menu.
    const openMenu = () => {
        document.getElementById("sideMenu").classList.add("open");

    }

    const closeMenu = () => {
        document.getElementById("sideMenu").classList.remove("open");
    }

    //Select new chat
    const newChat = () => {
        setThreadId(0);
        setCurrentChat([]);

    }


    const navigateToDashboard = () => {
        navigate('/dashboard', { replace: true });
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
                    <button onClick={newChat}>New chat</button>


                    {chatlog?.histories?.map((item) => {
                        return <button onClick={() => changeChat(item)}>{item.id}</button>;
                    })}
                </div>

                <button onClick={navigateToDashboard}>Go to dashboard</button>

                <button onClick={openMenu}>Select chat logs</button>

                <div>
                    {currentChat?.map((item: any) => {
                        return <ChatMessenge item={item} />;
                    })}
                    <div style={{ display: "flex", justifyContent: "center" }}>
                        {listening ? (
                            <button onClick={SpeechRecognition.stopListening}>Stop</button>
                        ) : (
                            <div>
                                <button onClick={() => SpeechRecognition.startListening({ language: 'en-US' })}>Start listening</button>
                                    <button onClick={() => {
                                        //reset the transript.
                                        resetTranscript();
                                        //reset the query.
                                        setQuery("");

                                    }}>Reset</button>
                            </div>
                        )}
                        <textarea 
                            id="inputField"
                            type="text" 
                            value={transcript || query} 
                            onChange={e => setQuery(e.target.value)} 
                            style={{
                                minWidth: '200px',
                                width: '100%',
                                minHeight: '40px',
                                resize: 'none',
                                overflow: 'hidden',
                                lineHeight: '1.5',
                                padding: '8px',
                                fontSize: '16px',
                                boxSizing: 'border-box',
                                transition: 'height 0.2s ease'
                            }}
                        />
                        <button id="queryButton" onClick={() => sendQuery(transcript || query)}>Send query!</button>
                    </div>
                    <p>{listening ? 'Microphone is on' : ''}</p>

                </div>

                


            </div>
        )
    }
}

export default ChatView;