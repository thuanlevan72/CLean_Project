import React from "react";
import { motion } from "framer-motion";
import Player from "./Player";
import Card from "./Card";

const GameTable = () => {
  // Sample data for players
  const players = [
    {
      id: 1,
      name: "Bạn",
      avatar: "👤",
      cardCount: 13,
      position: "bottom",
      isMainPlayer: true,
    },
    {
      id: 2,
      name: "Đối thủ 1",
      avatar: "🤖",
      cardCount: 13,
      position: "right",
      isMainPlayer: false,
    },
    {
      id: 3,
      name: "Đối thủ 2",
      avatar: "🤖",
      cardCount: 13,
      position: "top",
      isMainPlayer: false,
    },
    {
      id: 4,
      name: "Đối thủ 3",
      avatar: "🤖",
      cardCount: 13,
      position: "left",
      isMainPlayer: false,
    },
  ];

  // Sample cards for the center (played cards)
  const playedCards = [
    { id: 1, suit: "hearts", value: "A" },
    { id: 2, suit: "spades", value: "K" },
    { id: 3, suit: "diamonds", value: "Q" },
  ];

  // Sample cards for main player's hand
  const mainPlayerCards = [
    { id: 1, suit: "hearts", value: "3" },
    { id: 2, suit: "hearts", value: "4" },
    { id: 3, suit: "hearts", value: "5" },
    { id: 4, suit: "spades", value: "6" },
    { id: 5, suit: "diamonds", value: "7" },
    { id: 6, suit: "clubs", value: "8" },
    { id: 7, suit: "hearts", value: "9" },
    { id: 8, suit: "spades", value: "10" },
    { id: 9, suit: "diamonds", value: "J" },
    { id: 10, suit: "clubs", value: "Q" },
    { id: 11, suit: "hearts", value: "K" },
    { id: 12, suit: "spades", value: "A" },
    { id: 13, suit: "diamonds", value: "2" },
  ];

  const getPlayerByPosition = (position) => {
    return players.find((player) => player.position === position);
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-gray-900 to-gray-800 p-4">
      <motion.div
        initial={{ scale: 0.8, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        transition={{ duration: 0.5 }}
        className="relative w-full max-w-4xl aspect-[4/3] rounded-[50px] shadow-2xl overflow-hidden"
        style={{
          background:
            "linear-gradient(145deg, #1a5f2a 0%, #0d3d1a 50%, #1a5f2a 100%)",
          boxShadow:
            "0 0 60px rgba(0, 0, 0, 0.5), inset 0 0 100px rgba(0, 0, 0, 0.3)",
        }}>
        {/* Table felt texture overlay */}
        <div
          className="absolute inset-0 opacity-20"
          style={{
            backgroundImage:
              "radial-gradient(circle at 20% 30%, rgba(255,255,255,0.1) 0%, transparent 50%), radial-gradient(circle at 80% 70%, rgba(255,255,255,0.1) 0%, transparent 50%)",
          }}
        />

        {/* Table border */}
        <div
          className="absolute inset-0 rounded-[50px] border-8 border-amber-900"
          style={{
            boxShadow: "inset 0 0 20px rgba(0, 0, 0, 0.5)",
          }}
        />

        {/* Top player */}
        <div className="absolute top-4 left-1/2 transform -translate-x-1/2">
          <Player player={getPlayerByPosition("top")} />
        </div>

        {/* Left player */}
        <div className="absolute left-4 top-1/2 transform -translate-y-1/2">
          <Player player={getPlayerByPosition("left")} />
        </div>

        {/* Right player */}
        <div className="absolute right-4 top-1/2 transform -translate-y-1/2">
          <Player player={getPlayerByPosition("right")} />
        </div>

        {/* Center area - Played cards */}
        <motion.div
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.3, duration: 0.5 }}
          className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 flex gap-2">
          {playedCards.map((card, index) => (
            <motion.div
              key={card.id}
              initial={{ scale: 0, rotate: -180 }}
              animate={{ scale: 1, rotate: 0 }}
              transition={{
                delay: 0.4 + index * 0.1,
                type: "spring",
                stiffness: 200,
              }}>
              <Card card={card} isPlayed={true} />
            </motion.div>
          ))}
        </motion.div>

        {/* Bottom player - Main player */}
        <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2">
          <Player player={getPlayerByPosition("bottom")} />
        </div>

        {/* Main player's cards */}
        <motion.div
          initial={{ y: 50, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.5, duration: 0.5 }}
          className="absolute bottom-24 left-1/2 transform -translate-x-1/2 flex flex-wrap justify-center gap-1 max-w-2xl">
          {mainPlayerCards.map((card, index) => (
            <motion.div
              key={card.id}
              initial={{ y: 20, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              transition={{ delay: 0.6 + index * 0.05 }}
              whileHover={{ y: -10, scale: 1.1 }}
              className="cursor-pointer">
              <Card card={card} isPlayable={true} />
            </motion.div>
          ))}
        </motion.div>

        {/* Game title */}
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.8 }}
          className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 pointer-events-none">
          <h1 className="text-4xl font-bold text-yellow-400 opacity-30 text-center whitespace-nowrap">
            Tiến Lên Miền Nam
          </h1>
        </motion.div>
      </motion.div>
    </div>
  );
};

export default GameTable;
