import React from "react";
import { motion } from "framer-motion";
import { User } from "lucide-react";

const Player = ({ player }) => {
  if (!player) return null;

  const isVertical = player.position === "left" || player.position === "right";

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.8 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.3 }}
      className={`flex ${isVertical ? "flex-col items-center" : "flex-row items-center gap-3"}`}>
      {/* Avatar */}
      <motion.div
        whileHover={{ scale: 1.1 }}
        className={`relative ${isVertical ? "mb-2" : ""}`}>
        <div
          className={`flex items-center justify-center rounded-full bg-gradient-to-br from-amber-400 to-amber-600 shadow-lg ${
            player.isMainPlayer ? "w-16 h-16" : "w-12 h-12"
          }`}>
          {player.avatar === "👤" ? (
            <User
              className={`${player.isMainPlayer ? "w-8 h-8" : "w-6 h-6"} text-white`}
            />
          ) : (
            <span className={`${player.isMainPlayer ? "text-2xl" : "text-xl"}`}>
              {player.avatar}
            </span>
          )}
        </div>
        {/* Online indicator */}
        <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-green-500 rounded-full border-2 border-green-800" />
      </motion.div>

      {/* Player info */}
      <div className={`text-center ${isVertical ? "" : "text-left"}`}>
        <p
          className={`font-semibold text-white ${player.isMainPlayer ? "text-lg" : "text-sm"}`}>
          {player.name}
        </p>
        <div className="flex items-center justify-center gap-1 mt-1">
          <span className="text-xs text-gray-300">Bài:</span>
          <motion.span
            key={player.cardCount}
            initial={{ scale: 1.3 }}
            animate={{ scale: 1 }}
            className={`font-bold ${player.isMainPlayer ? "text-yellow-400" : "text-amber-400"}`}>
            {player.cardCount}
          </motion.span>
        </div>
      </div>

      {/* Card count badge for non-main players */}
      {!player.isMainPlayer && (
        <motion.div
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          transition={{ delay: 0.2 }}
          className={`${
            isVertical ? "mt-2" : "ml-2"
          } bg-red-600 text-white text-xs font-bold rounded-full w-6 h-6 flex items-center justify-center shadow-md`}>
          {player.cardCount}
        </motion.div>
      )}
    </motion.div>
  );
};

export default Player;
