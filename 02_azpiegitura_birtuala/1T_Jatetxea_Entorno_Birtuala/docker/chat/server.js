const express = require('express');
const http = require('http');
const { Server } = require('socket.io');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

const server = http.createServer(app);
const io = new Server(server, {
  cors: {
    origin: '*',
    methods: ['GET', 'POST']
  }
});

const PORT = process.env.PORT || 3001;
const activeUsers = new Map();

// HTTP Endpoints
app.get('/health', (req, res) => {
  res.json({
    status: 'ok',
    zerbitzaria: 'Jatetxea Chat Zerbitzaria',
    konektatutakoak: io.engine.clientsCount,
    denbora: new Date().toISOString()
  });
});

app.post('/api/notify', (req, res) => {
  const { gela, mezua, mota, datuak } = req.body;
  if (!mezua) {
    return res.status(400).json({ error: 'mezua derrigorrezkoa da' });
  }

  const notification = {
    mezua,
    mota: mota || 'info',
    datuak: datuak || null,
    denbora: new Date().toISOString()
  };

  if (gela && gela !== 'orokorra') {
    io.to(gela).emit('jakinarazpena', notification);
  } else {
    io.emit('jakinarazpena', notification);
  }

  res.json({ status: 'success', jakinarazpena: notification });
});

// WebSocket Events
io.on('connection', (socket) => {
  console.log(`Bezero berria konektatuta: ${socket.id}`);

  socket.on('sartu_gelan', ({ erabiltzailea, gela }) => {
    socket.join(gela);
    activeUsers.set(socket.id, { erabiltzailea, gela });
    io.to(gela).emit('erabiltzaile_aldaketa', {
      mota: 'sartu',
      erabiltzailea,
      gela,
      denbora: new Date().toISOString()
    });
  });

  socket.on('bidali_mezua', ({ gela, erabiltzailea, testua }) => {
    const payload = {
      id: `${Date.now()}-${Math.random().toString(36).substr(2, 5)}`,
      erabiltzailea,
      testua,
      gela,
      denbora: new Date().toISOString()
    };
    io.to(gela).emit('mezu_berria', payload);
  });

  socket.on('disconnect', () => {
    const user = activeUsers.get(socket.id);
    if (user) {
      io.to(user.gela).emit('erabiltzaile_aldaketa', {
        mota: 'irten',
        erabiltzailea: user.erabiltzailea,
        gela: user.gela,
        denbora: new Date().toISOString()
      });
      activeUsers.delete(socket.id);
    }
    console.log(`Bezeroa deskonektatuta: ${socket.id}`);
  });
});

server.listen(PORT, '0.0.0.0', () => {
  console.log(`Jatetxea Chat Zerbitzaria martxan dago portu honetan: ${PORT}`);
});
