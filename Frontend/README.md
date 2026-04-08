# Todo App

A simple React todo application with add and complete functionality.

## Features

- Add new todos
- Mark todos as complete
- Delete todos
- View todo statistics (total, completed, pending)

## Prerequisites

- Node.js (version 14 or higher)
- npm (comes with Node.js)

## Installation

1. Navigate to the Frontend directory:

   ```bash
   cd Frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

## Running the Application

1. Start the development server:

   ```bash
   npm start
   ```

2. Open your browser and navigate to:
   ```
   http://localhost:3000
   ```

## Building for Production

To create a production build:

```bash
npm run build
```

The build files will be created in the `build` directory.

## Project Structure

```
Frontend/
├── public/
│   └── index.html          # HTML template
├── src/
│   ├── components/
│   │   ├── AddTodo.js      # Component for adding new todos
│   │   ├── AddTodo.css     # Styling for AddTodo component
│   │   ├── TodoItem.js     # Component for individual todo items
│   │   ├── TodoItem.css    # Styling for TodoItem component
│   │   ├── TodoList.js     # Component for displaying todo list
│   │   └── TodoList.css    # Styling for TodoList component
│   ├── App.js              # Main App component with state management
│   ├── App.css             # Styling for App component
│   ├── index.js            # Entry point
│   └── index.css           # Global styles
├── package.json            # Project dependencies and scripts
└── README.md               # This file
```

## Usage

1. **Adding a Todo**: Type your todo in the input field and click "Add" or press Enter
2. **Completing a Todo**: Click the checkbox next to a todo to mark it as complete
3. **Deleting a Todo**: Hover over a todo and click the red × button to delete it

## Technologies Used

- React 18
- CSS3 with modern features (flexbox, gradients, transitions)
