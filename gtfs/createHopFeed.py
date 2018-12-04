import transitfeed
import csv
from pprint import pprint

def readScheduleFromCSV(file, route, route_stops, headsign, service_period):
    with open(file) as csv_file:
        csv_reader = csv.reader(csv_file, delimiter=',')
        line_count = 0
        for row in csv_reader:
            if line_count == 0:
                # print 'Column names are {}'.format(row)
                line_count += 1
            else:
                trip = route.AddTrip(service_period=service_period, headsign=headsign)
                for i in range(0,len(row)):
                    trip.AddStopTime(route_stops[i], stop_time=row[i])
                line_count += 1
        print 'Processed {} lines from {}.'.format(line_count, csv_file)

schedule = transitfeed.Schedule()
agency = schedule.AddAgency("Hop MKE", "http://www.thehopmke.com",
                    "America/Chicago", agency_id="HOPMKE")

intermodal_stop = schedule.AddStop(lat=43.03515, lng=-87.91609, name="Intermodal Station")
stpaul_wb_stop = schedule.AddStop(lat=43.03478, lng=-87.91169, name="Saint Paul at Plankinton Westbound")
stpaul_eb_stop = schedule.AddStop(lat=43.03454, lng=-87.91102, name="Saint Paul at Plankinton Eastbound")
thirdward_wb_stop = schedule.AddStop(lat=43.03508, lng=-87.9081, name="Historic Third Ward Westbound")
thirdward_eb_stop = schedule.AddStop(lat=43.03502, lng=-87.90675, name="Historic Third Ward Eastbound")
wisconsin_sb_stop = schedule.AddStop(lat=43.03815, lng=-87.90788, name="Wisconsin Avenue Southbound")
wisconsin_nb_stop = schedule.AddStop(lat=43.03817, lng=-87.90659, name="Wisconsin Avenue Northbound")
cityhall_sb_stop = schedule.AddStop(lat=43.04127, lng=-87.90828, name="City Hall Southbound")
cityhall_nb_stop = schedule.AddStop(lat=43.0413, lng=-87.90697, name="City Hall Northbound")
cathedral_wb_stop = schedule.AddStop(lat=43.0426, lng=-87.90511, name="Cathedral Square Westbound")
cathedral_eb_stop = schedule.AddStop(lat=43.04256, lng=-87.90511, name="Cathedral Square Eastbound")
jacksonjuneau_sb_stop = schedule.AddStop(lat=43.04558, lng=-87.90491, name="Jackson at Juneau Southbound")
jacksonjuneau_nb_stop = schedule.AddStop(lat=43.04584, lng=-87.90467, name="Jackson at Juneau Northbound")
ogdenjackson_wb_stop = schedule.AddStop(lat=43.04802, lng=-87.90482, name="Ogden/Jackson Westbound")
ogdenjackson_eb_stop = schedule.AddStop(lat=43.04824, lng=-87.90376, name="Ogden/Jackson Eastbound")
ogdenastor_wb_stop = schedule.AddStop(lat=43.04821, lng=-87.90008, name="Ogden at Astor Westbound")
ogdenastor_eb_stop = schedule.AddStop(lat=43.04805, lng=-87.90009, name="Ogden at Astor Eastbound")
burns_stop = schedule.AddStop(lat=43.04761, lng=-87.89581, name="Burns Commons")

service_period = schedule.GetDefaultServicePeriod()
service_period.SetStartDate("20181101")
service_period.SetEndDate("20191101")
service_period.SetWeekdayService(True)
# pprint(vars(service_period))

weekend_service_period = schedule.NewDefaultServicePeriod()
weekend_service_period.SetStartDate("20181101")
weekend_service_period.SetEndDate("20191101")
weekend_service_period.SetWeekendService(True)
# pprint(vars(weekend_service_period))

saturday_service_period = schedule.NewDefaultServicePeriod()
saturday_service_period.SetStartDate("20181101")
saturday_service_period.SetEndDate("20191101")
saturday_service_period.SetDayOfWeekHasService(5,True)
# pprint(vars(weekend_service_period))

schedule.SetDefaultServicePeriod(service_period)



# M-Line Northbound Weekday Route
route = schedule.AddRoute(short_name="MNBWD", long_name="M-Line Northbound", route_type=0)
route_stops = [intermodal_stop,
                stpaul_eb_stop,
                thirdward_eb_stop,
                wisconsin_nb_stop,
                cityhall_nb_stop,
                cathedral_eb_stop,
                jacksonjuneau_nb_stop, 
                ogdenjackson_eb_stop, 
                ogdenastor_eb_stop,
                burns_stop]

readScheduleFromCSV('weekday-nb.csv', route, route_stops, "To Burns Commons", service_period)

# M-Line Northbound Weekend Route
readScheduleFromCSV('weekend-nb.csv', route, route_stops, "To Burns Commons", weekend_service_period)

# M-Line Northbound Saturday Route
readScheduleFromCSV('saturday-nb.csv', route, route_stops, "To Burns Commons", saturday_service_period)

# M-Line Southbound Weekday Route
route = schedule.AddRoute(short_name="MSBWD", long_name="M-Line Southbound", route_type=0)
route_stops = [burns_stop,
                ogdenastor_wb_stop,
                ogdenjackson_wb_stop,
                jacksonjuneau_sb_stop,
                cathedral_wb_stop,
                cityhall_sb_stop,
                wisconsin_sb_stop,
                thirdward_wb_stop,
                stpaul_wb_stop,
                intermodal_stop]

readScheduleFromCSV('weekday-sb.csv', route, route_stops, "To Intermodal Station", service_period)

# M-Line Southbound Weekend Route
readScheduleFromCSV('weekend-sb.csv', route, route_stops, "To Intermodal Station", weekend_service_period)

# M-Line Southbound Saturday Route
readScheduleFromCSV('saturday-sb.csv', route, route_stops, "To Intermodal Station", saturday_service_period)

schedule.Validate()
schedule.WriteGoogleTransitFeed('hop-mke-gtfs.zip')




