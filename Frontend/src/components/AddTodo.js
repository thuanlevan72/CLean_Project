import React, { useState } from "react";
import "./AddTodo.css";

function AddTodo({ onAdd }) {
  const [text, setText] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (text.trim()) {
      onAdd(text.trim());
      setText("");
    }
  };

  return (
    <form onSubmit={handleSubmit} className="add-todo-form">
      <input
        type="text"
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder="Add a new todo..."
        className="add-todo-input"
      />
      <button type="submit" className="add-todo-button">
        Add
      </button>
    </form>
  );
}

export default AddTodo;
