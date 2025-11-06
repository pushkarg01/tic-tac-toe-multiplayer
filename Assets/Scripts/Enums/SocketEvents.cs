using System.Collections.Generic;
using System.Linq;
namespace W3.Tambola.Enums
{
    public enum SocketEvents
    {
        CONNECTED,
     //   PING,
        PONG,
        GAME_ERROR,
        //ENTER_ROOM,
       // ROOM_JOINED,
        START_GAME,
        //TICKET_P,
        //ROOM_UPDATE,
        GAME_STARTED,
        YOUR_TICKETS,
        //DRAW_NUMBER,
        //NUMBER_CALLED,
        //CLAIM,
        NEW_TICKET,
        JOIN_LOBBY,
        UPDATE_LOBBY,
        EXIT_LOBBY,
        //GAME_COMPLETED,
        //PLAYER_LEFT,
        //PLAYER_RECONNECTED,
        //ROOM_CLOSED,
    }

    public class SocketEventMapping
    {
        //Events Mapping
        public static readonly Dictionary<SocketEvents, string> EnumsToString = new Dictionary<SocketEvents, string>
        {
            {SocketEvents.CONNECTED, "CONNECTED"},
          //  {SocketEvents.PING, "PING"},
            {SocketEvents.PONG, "PONG"},
            {SocketEvents.GAME_ERROR, "GAME_ERROR" },
            //{SocketEvents.ENTER_ROOM, "ENTER_ROOM"},
            //{SocketEvents.ROOM_JOINED, "ROOM_JOINED"},
            {SocketEvents.START_GAME, "START_GAME"},
            //{SocketEvents.TICKET_P, "TICKET_P"},
            //{SocketEvents.ROOM_UPDATE, "ROOM_UPDATE"},
            {SocketEvents.GAME_STARTED, "GAME_STARTED"},
            {SocketEvents.YOUR_TICKETS, "YOUR_TICKETS"},
            //{SocketEvents.DRAW_NUMBER, "DRAW_NUMBER"},
            //{SocketEvents.NUMBER_CALLED, "NUMBER_CALLED"},
            //{SocketEvents.CLAIM, "CLAIM"},
            {SocketEvents.NEW_TICKET, "NEW_TICKET"},
            {SocketEvents.JOIN_LOBBY, "JOIN_LOBBY"},
            {SocketEvents.UPDATE_LOBBY, "UPDATE_LOBBY"},
            {SocketEvents.EXIT_LOBBY, "EXIT_LOBBY"},
            //{SocketEvents.GAME_COMPLETED, "GAME_COMPLETED"},
            //{SocketEvents.PLAYER_LEFT, "PLAYER_LEFT"},
            //{SocketEvents.PLAYER_RECONNECTED, "PLAYER_RECONNECTED"},
            //{SocketEvents.ROOM_CLOSED, "ROOM_CLOSED"},
        };
        
        //Changes Enums to String
        public static string GetString(SocketEvents events)
        {
            return EnumsToString.GetValueOrDefault(events);
        }
        
        //Returns Enum From Dictionary against key
        public static SocketEvents GetEnum(string socketEvent)
        {
            return EnumsToString.FirstOrDefault(x => x.Value.Equals(socketEvent)).Key;
        }
    }
}