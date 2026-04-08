import React from "react";
import { motion } from "framer-motion";

const Card = ({ card, isPlayable = false, isPlayed = false }) => {
  const getSuitColor = (suit) => {
    return suit === "hearts" || suit === "diamonds"
      ? "text-red-600"
      : "text-gray-900";
  };

  const getSuitSymbol = (suit) => {
    const symbols = {
      hearts: "♥",
      diamonds: "♦",
      clubs: "♣",
      spades: "♠",
    };
    return symbols[suit] || "";
  };

  const getSuitBgColor = (suit) => {
    return suit === "hearts" || suit === "diamonds"
      ? "bg-gradient-to-br from-red-50 to-red-100"
      : "bg-gradient-to-br from-gray-50 to-gray-100";
  };

  return (
    <motion.div
      whileHover={isPlayable ? { scale: 1.05, y: -5 } : {}}
      whileTap={isPlayable ? { scale: 0.95 } : {}}
      className={`relative ${isPlayed ? "w-16 h-24" : "w-12 h-18"} rounded-lg shadow-lg cursor-pointer transition-shadow ${
        isPlayable ? "hover:shadow-xl" : ""
      }`}
      style={{
        perspective: "1000px",
      }}>
      {/* Card front */}
      <div
        className={`absolute inset-0 ${getSuitBgColor(card.suit)} rounded-lg border border-gray-200 flex flex-col items-center justify-center`}
        style={{
          backfaceVisibility: "hidden",
          transform: "rotateY(0deg)",
        }}>
        {/* Top left value and suit */}
        <div
          className={`absolute top-1 left-1 ${getSuitColor(card.suit)} font-bold ${isPlayed ? "text-lg" : "text-sm"}`}>
          <div>{card.value}</div>
          <div className={`${isPlayed ? "text-xl" : "text-xs"}`}>
            {getSuitSymbol(card.suit)}
          </div>
        </div>

        {/* Center suit symbol */}
        <div
          className={`${getSuitColor(card.suit)} ${isPlayed ? "text-4xl" : "text-2xl"} opacity-80`}>
          {getSuitSymbol(card.suit)}
        </div>

        {/* Bottom right value and suit (inverted) */}
        <div
          className={`absolute bottom-1 right-1 ${getSuitColor(card.suit)} font-bold ${isPlayed ? "text-lg" : "text-sm"} transform rotate-180`}>
          <div>{card.value}</div>
          <div className={`${isPlayed ? "text-xl" : "text-xs"}`}>
            {getSuitSymbol(card.suit)}
          </div>
        </div>

        {/* Playable indicator */}
        {isPlayable && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="absolute inset-0 rounded-lg border-2 border-yellow-400 pointer-events-none"
            style={{
              boxShadow: "0 0 10px rgba(250, 204, 21, 0.5)",
            }}
          />
        )}
      </div>

      {/* Card back (for face-down cards) */}
      <div
        className="absolute inset-0 bg-gradient-to-br from-blue-600 to-blue-800 rounded-lg border border-blue-900 flex items-center justify-center"
        style={{
          backfaceVisibility: "hidden",
          transform: "rotateY(180deg)",
        }}>
        <div className="w-8 h-8 border-2 border-blue-300 rounded-full opacity-50" />
      </div>
    </motion.div>
  );
};

export default Card;
