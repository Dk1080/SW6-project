
function ChatMessenge(props) {
    console.log(props.item);
    //Set the color and posistion of the messasge based on the role.
    let color:string,pos:string;
    if (props.item.role == "assistant") {
        color = "#d3d3d3";
        pos = "right"
    } else {
        color = "#add8e6";
        pos = "left"
    }

    return (

        <div className="ChatMessenge">
            <p style={{ backgroundColor: color, textAlign: pos }}>
                {props.item.text}
            </p>
        </div>

    )

}

export default ChatMessenge;