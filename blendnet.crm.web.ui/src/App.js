import React from 'react';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import Divider from '@material-ui/core/Divider';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import CssBaseline from '@material-ui/core/CssBaseline';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import AccountCircle from '@material-ui/icons/AccountCircle';
import MenuItem from "@material-ui/core/MenuItem";
import Menu from "@material-ui/core/Menu";
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import './custom.css';
import { ContentProviderList } from './components/ContentProviderList';
import { HubList } from './components/HubList';
import { Profile } from './components/User/Profile';
import { Route } from 'react-router';
import { Link } from 'react-router-dom';
import Avatar from '@material-ui/core/Avatar';

const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  root: {
    flexGrow: 1,
  },
  title: {
    flexGrow: 1
  },
  menuButton: {
    marginRight: theme.spacing(2),
  },
  hide: {
    display: 'none',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  drawerHeader: {
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(0, 1),
    // necessary for content to be below app bar
    ...theme.mixins.toolbar,
    justifyContent: 'flex-end',
  },
  link: {
    textDecoration: 'none',
  },
  container: {
    marginLeft: 'auto',
    marginRight: '15%',
    marginTop: '5%',
    width: '70%'
  }
}));


export default function TemporaryDrawer() {
  const classes = useStyles();
  const [open, setOpen] = React.useState(false);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const openAnchor = Boolean(anchorEl);
  const [state, setState] = React.useState({
    left: false
  });

  const toggleDrawer = (anchor, open) => (event) => {
    if (event.type === 'keydown' && (event.key === 'Tab' || event.key === 'Shift')) {
      return;
    }

    setState({ ...state, [anchor]: open });
  };

  const handleMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };


  const list = (anchor) => (
    <div
      className={classes.list}
      role="presentation"
      onClick={toggleDrawer(anchor, false)}
      onKeyDown={toggleDrawer(anchor, false)}
    >
        <div className={classes.drawerHeader}>
            <IconButton>
                <Avatar alt="Mishtu" src="/public/mishtulogo.png" />
            </IconButton>
            <IconButton onClick={toggleDrawer(anchor, false)}>
                <ChevronLeftIcon />
            </IconButton>
        </div>
        <Divider></Divider>
        <List>
            <ListItem button key='Content-Providers'>
              <ListItemText>
                  <Link to="/content-provide-list" className={classes.link}>Content-Providers</Link>
              </ListItemText>
            </ListItem>
            <ListItem button key='Hubs'>
                  <ListItemText>
                      <Link  className={classes.link} to="/hubs">Hubs</Link>
                  </ListItemText>
            </ListItem>
            <ListItem button key='Policies' >
                <ListItemText>
                    <Link className={classes.link} to="/">Policies</Link>
              </ListItemText>
            </ListItem>
        </List>
        <Divider></Divider>
    </div>
  );

  return (
    <div>
        <React.Fragment>
        <AppBar
          position="fixed"
        >
          <Toolbar>
            <IconButton
              color="inherit"
              aria-label="open drawer"
              onClick={toggleDrawer('left', true)}
              edge="start"
              className={clsx(classes.menuButton, open && classes.hide)}
            >
              <MenuIcon />
            </IconButton>
            <Typography variant="h6" noWrap className={classes.title}>
              Bine CRM
            </Typography>
            <div>
              <IconButton
                    aria-label="account of current user"
                    aria-controls="menu-appbar"
                    aria-haspopup="true"
                    onClick={handleMenu}
                    color="inherit"
                  >
                    <AccountCircle />
              </IconButton>
              <Menu
                id="menu-appbar"
                anchorEl={anchorEl}
                anchorOrigin={{
                  vertical: "top",
                  horizontal: "right"
                }}
                keepMounted
                transformOrigin={{
                  vertical: "top",
                  horizontal: "right"
                }}
                open={openAnchor}
                onClose={handleClose}
                >
                <MenuItem> <Link to="/profile" className={classes.link}>Profile</Link></MenuItem>
                <MenuItem onClick={handleClose}>My account</MenuItem>
              </Menu>
            </div>
          </Toolbar>
        </AppBar>
          {/* <Button onClick={toggleDrawer('left', true)}>Left</Button> */}
          <Drawer anchor='left' open={state['left']} onClose={toggleDrawer('left', false)}
          classes={{
            paper: classes.drawerPaper,
          }}>
            {list('left')}
          </Drawer>
        </React.Fragment>
        <div className={classes.drawerHeader} />
        <CssBaseline />
          <Container className={classes.container}>
            <Route path='/content-provide-list' component={ContentProviderList} />
            <Route path='/hubs' component={HubList} />
            <Route path='/profile' component={Profile} />
          </Container>
    </div>
  );
}
