import React from 'react';
import PropTypes from 'prop-types';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TablePagination from '@material-ui/core/TablePagination';
import TableRow from '@material-ui/core/TableRow';
import TableSortLabel from '@material-ui/core/TableSortLabel';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Checkbox from '@material-ui/core/Checkbox';
import { IconButton } from '@fluentui/react/lib/Button';
import { withStyles } from '@material-ui/core/styles';
import Tooltip from '@material-ui/core/Tooltip';
import Grid from '@material-ui/core/Grid';
import { BrowserRouter as Router, Route } from 'react-router-dom';
import { ContentProviderDetail } from './ContentProvider/ContentProviderDetail';


function createData(name, contact, address, status) {
  return { name, contact, address, status };
}

function populateData(contentProvidersList){
  // rows =[];
  // contentProvidersList.forEach(element => {
  //   rows.push(createData(element.name,
  //             element.contentAdministrators!=null?element.contentAdministrators[0]?.mobile:'',
  //             element.address.state,
  //             element.isActive));
  // });
}

const rows = [
  createData('Mishtu Mobile Booth', '+91 9231232341', 'J.P. nagar, Banglore', 'Active'),
  createData('Apple Care', '+91 9333232341', 'A.M. nagar, Banglore', 'Active'),
  createData('Dell Center', '+91 9451232341', 'Bilekahalli, Banglore', 'Active'),
  createData('Random Grocery store', '+91 9454532341', 'Richmond Circle, Banglore', 'Disabled'),
  createData('Random Tapri', '+91 9231244441', 'Pimpri, Pune','Active'),
  createData('Small business venture', '+91 9222329341', 'Worli, Mumbai','Disabled'),
  createData('Medium Business', '+91 5454433441', 'Delhi, India', 'Active'),
  createData('Blendnet Mobiles', '+91 8234232341', 'Whitefield, Banglore','Disabled'),
  createData('Samsung Smart booth', '+91 9444442341', 'Bellandur, Banglore', 'Active'),
  createData('Motorola Store', '+91 8888832341', 'Electronic city, Banglore', 'Active'),
  createData('Dmart', '+91 9231856233', 'Harlur road, Banglore', 'Disabled'),
  createData('Reliance', '+91 9245632341', 'Ranka colony, Banglore', 'Active'),
  createData('List Exhaused', '+91 8981232341', 'JayaNagar, Banglore', 'Disabled'),
];

// let rows = [];

function descendingComparator(a, b, orderBy) {
  if (b[orderBy] < a[orderBy]) {
    return -1;
  }
  if (b[orderBy] > a[orderBy]) {
    return 1;
  }
  return 0;
}

function getComparator(order, orderBy) {
  return order === 'desc'
    ? (a, b) => descendingComparator(a, b, orderBy)
    : (a, b) => -descendingComparator(a, b, orderBy);
}

function stableSort(array, comparator) {
  const stabilizedThis = array.map((el, index) => [el, index]);
  stabilizedThis.sort((a, b) => {
    const order = comparator(a[0], b[0]);
    if (order !== 0) return order;
    return a[1] - b[1];
  });
  return stabilizedThis.map((el) => el[0]);
}

const headCells = [
  { id: 'name', numeric: false, disablePadding: true, label: 'Name' },
  { id: 'contact', numeric: true, disablePadding: false, label: 'Contact' },
  { id: 'address', numeric: true, disablePadding: false, label: 'Address' },
  { id: 'status', numeric: true, disablePadding: false, label: 'Status' }
];

function EnhancedTableHead(props) {
  const { classes, onSelectAllClick, order, orderBy, numSelected, rowCount, onRequestSort } = props;
  const createSortHandler = (property) => (event) => {
    onRequestSort(event, property);
  };

  return (
    <TableHead>
      <TableRow>
          <TableCell padding="checkbox"></TableCell>
        <TableCell padding="checkbox">
          <MSCheckbox
            indeterminate={numSelected > 0 && numSelected < rowCount}
            checked={rowCount > 0 && numSelected === rowCount}
            onChange={onSelectAllClick}
            inputProps={{ 'aria-label': 'select all Content-Providers' }}
          />
        </TableCell>
        {headCells.map((headCell) => (
          <TableCell
            key={headCell.id}
            align={headCell.numeric ? 'right' : 'left'}
            padding={headCell.disablePadding ? 'none' : 'default'}
            sortDirection={orderBy === headCell.id ? order : false}
          >
            <TableSortLabel
              active={orderBy === headCell.id}
              direction={orderBy === headCell.id ? order : 'asc'}
              onClick={createSortHandler(headCell.id)}
            >
              {headCell.label}
              {orderBy === headCell.id ? (
                <span className={classes.visuallyHidden}>
                  {order === 'desc' ? 'sorted descending' : 'sorted ascending'}
                </span>
              ) : null}
            </TableSortLabel>
          </TableCell>
        ))}
      </TableRow>
    </TableHead>
  );
}

function showContentProviderDetail(){
  this.props.history.push('/content-provider-detail');
};

EnhancedTableHead.propTypes = {
  classes: PropTypes.object.isRequired,
  numSelected: PropTypes.number.isRequired,
  onRequestSort: PropTypes.func.isRequired,
  onSelectAllClick: PropTypes.func.isRequired,
  order: PropTypes.oneOf(['asc', 'desc']).isRequired,
  orderBy: PropTypes.string.isRequired,
  rowCount: PropTypes.number.isRequired,
};

const useToolbarStyles = makeStyles((theme) => ({
  root: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(1),
  },
  highlight:
        {
          color: "#00000",
          backgroundColor: "#b3b2b1",
        },
  title: {
    flex: '1 1 100%',
  },
}));



const EnhancedTableToolbar = (props) => {
  const classes = useToolbarStyles();
  const { numSelected } = props;

  return (
    <Toolbar
      className={clsx(classes.root, {
        [classes.highlight]: numSelected > 0,
      })}
    >
      {numSelected > 0 ? (
        <Typography className={classes.title} color="inherit" variant="subtitle1" component="div">
          {numSelected} selected
        </Typography>
      ) : (
        <Typography className={classes.title} variant="h6" id="tableTitle" component="div">
          Content Providers
        </Typography>
      )}

  
        {numSelected == 1 ? (
          <Grid container spacing={1}>
            <Grid item xs={8}>
            </Grid>
            <Grid item xs={2}>
              <Tooltip title="Edit">
                <IconButton title="Edit" ariaLabel="Edit"   iconProps={{ iconName: 'Edit' }} 
                onClick={(event) => showContentProviderDetail()}/>
              </Tooltip>
            </Grid>
            <Grid item xs={2}>
              <Tooltip title="Delete">
                <IconButton title="Delete" ariaLabel="Delete"   iconProps={{ iconName: 'Delete' }} />
              </Tooltip>
            </Grid>
          </Grid>
        ) : (<Grid  item xs={0}/>)}

        {numSelected > 1 ? (
           <Grid container spacing={1}>
             <Grid  item xs={10}/>
              <Grid item xs={1}>
                <Tooltip title="Delete">
                  <IconButton title="Delete" ariaLabel="Delete"   iconProps={{ iconName: 'Delete' }} />
                </Tooltip>
                </Grid>
            </Grid>
        ) :  (<Grid  item xs={0}/>)}

      {numSelected == 0 ? (
        <Grid container spacing={1}>
          <Grid  item xs={10}/>
          <Grid item xs={1}>
            <Tooltip title="Filter list">
              <IconButton title="Filter" ariaLabel="Filter"   iconProps={{ iconName: 'Filter' }} />
            </Tooltip>
          </Grid>
        </Grid>
          
        ) :   (<Grid  item xs={0}/>)}

   
    </Toolbar>
  );
};

EnhancedTableToolbar.propTypes = {
  numSelected: PropTypes.number.isRequired,
};

const useStyles = makeStyles((theme) => ({
  root: {
    width: '100%',
  },
  paper: {
    width: '100%',
    marginBottom: theme.spacing(2),
  },
  table: {
    minWidth: 750,
  },
  visuallyHidden: {
    border: 0,
    clip: 'rect(0 0 0 0)',
    height: 1,
    margin: -1,
    overflow: 'hidden',
    padding: 0,
    position: 'absolute',
    top: 20,
    width: 1,
  },
  tableContainer: {
    maxHeight: 320
  },
}));

const MSCheckbox = withStyles({
  root: {
    // color: '#0078d4',
    '&$checked': {
      color: '#0078d4',
    },
  },
  checked: {},
})((props) => <Checkbox color="default" {...props} />);

export default function EnhancedTable(props) {
  const classes = useStyles();
  const [order, setOrder] = React.useState('asc');
  const [orderBy, setOrderBy] = React.useState('name');
  const [selected, setSelected] = React.useState([]);
  const [page, setPage] = React.useState(0);
  const [rowsPerPage, setRowsPerPage] = React.useState(5);

  populateData(props.rows);

  const handleRequestSort = (event, property) => {
    const isAsc = orderBy === property && order === 'asc';
    setOrder(isAsc ? 'desc' : 'asc');
    setOrderBy(property);
  };

  const handleSelectAllClick = (event) => {
    if (event.target.checked) {
      const newSelecteds = rows.map((n) => n.name);
      setSelected(newSelecteds);
      return;
    }
    setSelected([]);
  };


  const handleClick = (event, name) => {
    const selectedIndex = selected.indexOf(name);
    let newSelected = [];

    if (selectedIndex === -1) {
      newSelected = newSelected.concat(selected, name);
    } else if (selectedIndex === 0) {
      newSelected = newSelected.concat(selected.slice(1));
    } else if (selectedIndex === selected.length - 1) {
      newSelected = newSelected.concat(selected.slice(0, -1));
    } else if (selectedIndex > 0) {
      newSelected = newSelected.concat(
        selected.slice(0, selectedIndex),
        selected.slice(selectedIndex + 1),
      );
    }

    setSelected(newSelected);
  };

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };


  const isSelected = (name) => selected.indexOf(name) !== -1;

  const emptyRows = rowsPerPage - Math.min(rowsPerPage, rows.length - page * rowsPerPage);

  return (
    <div>
      <Router>
        <Route path="/content-provider-detail" component={ContentProviderDetail} />
      </Router>
      <div className={classes.root}>
        <Paper className={classes.paper}>
          <EnhancedTableToolbar numSelected={selected.length} />
          <TableContainer className={classes.tableContainer}>
            <Table
              stickyHeader 
              className={classes.table}
              aria-labelledby="tableTitle"
              size='medium'
              aria-label="enhanced table"
            >
              <EnhancedTableHead
                classes={classes}
                numSelected={selected.length}
                order={order}
                orderBy={orderBy}
                onSelectAllClick={handleSelectAllClick}
                onRequestSort={handleRequestSort}
                rowCount={rows.length}
              />
              <TableBody>
                {stableSort(rows, getComparator(order, orderBy))
                  .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                  .map((row, index) => {
                    const isItemSelected = isSelected(row.name);
                    const labelId = `enhanced-table-checkbox-${index}`;

                    return (
                      <TableRow
                        hover
                        onClick={(event) => handleClick(event, row.name)}
                        role="checkbox"
                        aria-checked={isItemSelected}
                        tabIndex={-1}
                        key={row.name}
                        selected={isItemSelected}
                      >

                      <TableCell padding="checkbox"></TableCell>
                        <TableCell padding="checkbox">
                          <MSCheckbox
                            checked={isItemSelected}
                            inputProps={{ 'aria-labelledby': labelId }}
                          />
                        </TableCell>
                        <TableCell component="th" id={labelId} scope="row" padding="none">
                          {row.name}
                        </TableCell>
                        <TableCell align="right">{row.contact}</TableCell>
                        <TableCell align="right">{row.address}</TableCell>
                        <TableCell align="right">{row.status}</TableCell>
                      </TableRow>
                    );
                  })}
                {emptyRows > 0 && (
                  <TableRow style={{ height: 53 * emptyRows }}>
                    <TableCell colSpan={6} />
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
          <TablePagination
            rowsPerPageOptions={[5, 10, 25]}
            component="div"
            count={rows.length}
            rowsPerPage={rowsPerPage}
            page={page}
            onChangePage={handleChangePage}
            onChangeRowsPerPage={handleChangeRowsPerPage}
          />
        </Paper>
      </div>
    </div>
    
  );
}
